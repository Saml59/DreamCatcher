
from flask import Flask, request
import json, users_dao, chat_dao

from db import Class, db, User, Report, Message, Conversation
import os
from datetime import date

app = Flask(__name__)
db_filename = "dc.db"

app.config["SQLALCHEMY_DATABASE_URI"] = "sqlite:///%s" % db_filename
app.config["SQLALCHEMY_TRACK_MODIFICATIONS"] = False
app.config["SQLALCHEMY_ECHO"] = True

db.init_app(app)
with app.app_context():
    db.create_all()

def success_response(data, code=200):
    return json.dumps({"success": True, "data": data}), code

def failure_response(message, code=404):
    return json.dumps({"success": False, "error": message}), code

#extract token from a request
def extract_token(request) :
    auth_header = request.headers.get("Authorization")
    if auth_header is None :
        return False, "Missing Authorization header"
    bearer_token = auth_header.replace("Bearer ", "").strip()
    if bearer_token is None or not bearer_token :
        return False, "Invalid Authorization Header"

    return True, bearer_token

#verifies that the incoming request is coming from an administrator
def verify_administrator(request) :
    success_1, token = extract_token(request)
    if not success_1:
        return False
    success_2, user = users_dao.verify_session_token(token)
    if not success_2 :
        return False
    return user.role == "admin"

#checks the token and returns the user associated, or False if there is no user. Form is (boolean, user)
def verify_user(request) :
    success_1, token = extract_token(request)
    if not success_1:
        return False, None
    return users_dao.verify_session_token(token)

#creates a token dictionary
def create_token_data(st, se, rt) :
    return {
        "session token" : st,
        "session expiration" : se.isoformat(),
        "refresh token" : rt
    }

#ROUTES

#USER ROUTES

#Update a session
@app.route("/api/session/", methods=["POST"])
def update_session() :
    body = json.loads(request.data)
    refresh_token = body.get("refresh_token")
    success, session_token, session_expiration, refresh_token_new = users_dao.renew_session(refresh_token)
    if not success :
        return failure_response("Invalid refresh token")
    data = create_token_data(session_token, session_expiration, refresh_token_new)
    return success_response(data)

#Create a user
@app.route("/api/users/", methods=["POST"])
# Required data: first name, last name, role, date of birth
# Optional data: middle initial
def create_user() :
    body = json.loads(request.data)
    email = body.get("email")
    username = body.get("username")
    passhash = body.get("passhash")
    salt = body.get("salt")
    first_name = body.get("first_name")
    middle_initial = body.get("middle_initial")
    last_name = body.get("last_name")
    dob = body.get("dob") #ISO format
    role = body.get("role")
    required_fields = [email, username, passhash, salt, first_name, last_name, dob, role]
    #Checks to make sure user can be created without conflicts
    if not all(var is not None for var in required_fields) :
        return failure_response("Insufficient information")
    if (users_dao.get_user_by_email(email) is not None or users_dao.get_user_by_username(username) is not None) :
        return failure_response("User already exists!")
    year=int(dob[:4])
    month=int(dob[5:7])
    day=int(dob[8:10])
    date_of_birth = date(year, month, day)
    #Check to make sure that the given role is one of the allowed ones
    if role not in ["instructor", "student", "tutor"] :
        return failure_response("Invalid role")
    session_token, session_expiration, refresh_token = users_dao.create_user(
        email, username, passhash, salt, first_name, middle_initial, last_name, date_of_birth, role)
    data = create_token_data(session_token, session_expiration, refresh_token)
    return success_response(data, 201)

#Creates an admin (Sam) account that can then be used to change others' roles
@app.route("/api/users/sam/", methods=["POST"])
def create_admin_sam() :
    user = users_dao.get_user_by_username("syl59")
    if user is not None :
        return failure_response("Sam already exists")
    dob = date(2001, 1, 16)
    session_token, session_expiration, refresh_token = users_dao.create_user\
        ("syl59@cornell.edu","syl59","f8shfg89eh9tifnwe","None","Samuel", "Y", "Lee", dob, "admin")
    data = create_token_data(session_token, session_expiration, refresh_token)
    return success_response(data)

#Gets the salt for a given username, to eventually be used to hash the password and log in
@app.route("/api/users/salt/")
def get_salt() :
    body = json.loads(request.data)
    username = body.get("username")
    user = users_dao.get_user_by_username(username)
    if user is None :
        return failure_response("No user found")
    else :
        return user.get_salt()

#Login
@app.route("/api/login/", methods=["POST"])
def login() :
    body = json.loads(request.data)
    username = body.get("username")
    passhash = body.get("passhash")
    success, session_token, session_expiration, refresh_token = users_dao.verify_credentials(username, passhash)
    if not success :
        return failure_response("Username or Password is invalid")
    else :
        db.session.commit()
        data = {
            "session token": session_token,
            "session expiration": session_expiration.isoformat(),
            "refresh token": refresh_token
        }
        return success_response(data)


#Get a user with a given user_id
@app.route("/api/users/<int:user_id>/")
def get_user_by_id(user_id):
    success, accessing_user = verify_user(request)
    if not success :
        return failure_response("No Authorization")
    user = users_dao.get_user_by_id(user_id)
    if user is None:
        return failure_response("User not found!")
    return success_response(user.serialize())

#Search for users by last name, first and middle names optional
@app.route("/api/users/")
def get_users_by_name():
    success, accessing_user = verify_user(request)
    if not success:
        return failure_response("No Authorization")
    body = json.loads(request.data)
    first_name = body.get("first_name")
    last_name = body.get("last_name")
    middle_initial = body.get("middle_initial")
    success, users = users_dao.get_users_by_name(first_name=first_name, middle_initial=middle_initial, last_name=last_name)
    if (not success) :
        return failure_response("No name given")
    if (len(users) == 0):
        return failure_response("No users found with given name")
    userlist = [user.serialize() for user in users]
    return success_response(userlist)


#delete a user by id (admin)
@app.route("/api/users/<int:user_id>/", methods=["DELETE"])
def delete_user_by_id(user_id) :
    admin = verify_administrator(request)
    if not admin:
        return failure_response("Insufficient permissions")
    user = users_dao.get_user_by_id(user_id)
    if (user is None) :
        return failure_response("User not found")
    db.session.delete(user)
    db.session.commit()
    return success_response(user.serialize())

#change the role of a user (admin)
@app.route("/api/users/<int:user_id>/", methods=["POST"])
def assign_role(user_id) :
    body = json.loads(request.data)
    admin = verify_administrator(request)
    if not admin :
        return failure_response("Insufficient Permissions")
    new_role = body.get("role")
    if new_role is None :
        return failure_response("No new role given")
    success_2, user = users_dao.verify_session_token(request)
    if not success_2 :
        return failure_response("Invalid Token")
    update_user = users_dao.get_user_by_id(user_id)
    if update_user is None :
        return failure_response("User not found")
    update_user.role = new_role
    db.session.commit()
    return success_response(update_user.serialize())

#TUTOR ROUTES

#Assign a tutor to a student (admin)
@app.route("/api/tutors/<int:user_id>/", methods=["POST"])
def assign_tutor(user_id) :
    admin = verify_administrator(request)
    if not admin :
        return failure_response("Insufficient permissions")
    body=json.loads(request.data)
    student_id = body.get("student_id")
    if (student_id is None) :
        return failure_response("No student given")
    tutor = users_dao.get_user_by_id(user_id)
    student = users_dao.get_user_by_id(student_id)
    if (tutor is None or student is None) :
        return failure_response("Tutor or Student does not exist")
    tutor.students.append(student)
    db.session.commit()
    return success_response(tutor.serialize())

#gets the tutor of a given user
@app.route("/api/users/<int:user_id>/tutor/")
def get_tutor(user_id) :
    success, accessing_user = verify_user(request)
    if not success:
        return failure_response("No Authorization")
    student = users_dao.get_student_by_id(user_id)
    if student is None :
        return failure_response("Student not found")
    tutor_id = student.get_tutor()
    if tutor_id is None :
        return failure_response("Student has no tutor")
    return success_response(users_dao.get_user_by_id(tutor_id).external_serialize())

#Submits a report with the accessing user as the tutor
@app.route("/api/report/<int:user_id>/", methods=["POST"])
def submit_report(user_id) :
    success, tutor = verify_user(request)
    body = json.loads(request)
    message = body.get("report_message")
    if message is None :
        return failure_response("No report given")
    if not success :
        return failure_response("Invalid Authorization")
    student = users_dao.get_user_by_id(user_id)
    if student is None :
        return failure_response("Student not found")
    report = Report(
        tutor_id=tutor.id,
        student_id=student.id,
        report=message
    )
    db.session.add(report)
    db.session.commit()
    return success_response(report.serialize())

#CLASS ROUTES

#Creates a class (admin)
@app.route("/api/class/create/", methods=["POST"])
def create_class() :
    admin = verify_administrator(request)
    if not admin :
        return failure_response("Insufficient permissions")
    body = json.loads(request.data)
    name = body.get("class_name")
    created_class = Class(name=name)
    db.session.add(created_class)
    db.session.commit()
    return success_response(created_class.serialize())

#Add a user to a class (admina)
@app.route("/api/class/<int:class_id>/", methods=["POST"])
def add_user_to_class(class_id) :
    admin = verify_administrator(request)
    if not admin :
        return failure_response("Insufficient permissions")
    body=json.loads(request.data)
    user_id = body.get("user_id")
    user = users_dao.get_user_by_id(user_id)
    if user is None :
        return failure_response("User not found")
    user.class_id = class_id
    db.session.commit()
    return success_response(user.serialize())

#Gets the class roster
@app.route("/api/class/<int:class_id>/")
def get_roster(class_id) :
    success, user = verify_user(request)
    if not success :
        return failure_response("Invalid authorization")
    course = Class.query.filter_by(id=class_id).first()
    if course is None :
        return failure_response("Class not found")
    return course.serialize()

#Gets all classes
@app.route("/api/class/")
def get_classes() :
    success, user = verify_user(request)
    if not success:
        return failure_response("Invalid authorization")
    classes = Class.query.all()
    if len(classes = 0) :
        return failure_response("No classes")
    return success_response([c.serialize() for c in classes])


#MESSAGING ROUTES

#Sends a message to another user
@app.route("/api/chat/<int:recipient_id>/", methods=["POST"])
def send_message(recipient_id) :
    success, sender = verify_user(request)
    if not success :
        return failure_response("Invalid authorization")
    body = json.loads(request.data)
    message_body = body.get("message_body")
    sender_id = sender.id
    exists, convo = chat_dao.check_conversation_exists(sender_id, recipient_id)
    if not exists :
        convo = chat_dao.create_conversation(sender_id, recipient_id)
    message = chat_dao.create_message(sender_id=sender_id, recipient_id=recipient_id, body=message_body, conversation_id=convo.id)
    return success_response(message.serialize())

#gets a conversation between two users
@app.route("/api/chat/<int:user_2_id>/")
def get_chat(user_2_id) :
    success, user_1 = verify_user(request)
    if not success :
        return failure_response("Invalid authorization")
    user_1_id = user_1.id
    exists, convo = chat_dao.check_conversation_exists(user_1_id, user_2_id)
    if not exists :
        return failure_response("Conversation does not exist")
    message_list = list(m.serialize() for m in convo.messages)
    return success_response(message_list)


if __name__ == "__main__":
    port = int(os.environ.get("PORT", 5000))
    app.run(host='0.0.0.0', port=port)
