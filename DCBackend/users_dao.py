from db import User, Instructor, Tutor, Student, db, Administrator


def get_user_by_id(id) :
    return User.query.filter_by(id=id).first()

def get_student_by_id(id) :
    return Student.query.filter_by(id=id).first()

def get_user_by_email(email) :
    return User.query.filter_by(email=email).first()

def get_user_by_username(username) :
    return User.query.filter_by(username=username).first()

#returns success (True or False), users (list)
def get_users_by_name(**kwargs) :
    first_name = kwargs.get("first_name")
    last_name = kwargs.get("last_name")
    middle_initial = kwargs.get("middle_initial")
    if (middle_initial is None) :
        if (first_name is None):
            if (last_name is None) :
                return False, None
            return True, User.query.filter_by(last_name=last_name).all()
        else :
            if (last_name is None) :
                return True, User.query.filter_by(first_name=first_name).all()
            else :
                return True, User.query.filter_by(first_name=first_name, last_name=last_name).all()
    else :
        if (first_name is None):
            if (last_name is None) :
                return True, User.query.filter_by(middle_initial=middle_initial).all()
            return True, User.query.filter_by(middle_initial=middle_initial, last_name=last_name).all()
        return True, User.query.filter_by(first_name=first_name, middle_initial=middle_initial, last_name=last_name).all()


def get_user_by_session_token(session_token) :
    return User.query.filter_by(session_token=session_token).first()

def get_user_by_refresh_token(refresh_token) :
    return User.query.filter_by(refresh_token=refresh_token).first()

#verify that the session token is legitimate, returns True/False, user
def verify_session_token(session_token) :
    user = get_user_by_session_token(session_token)
    if (user is None) :
        return False, None
    return user.verify_session_token(session_token), user

#renews the session with new tokens
def renew_session(refresh_token) :
    user = get_user_by_refresh_token(refresh_token)
    if user is None :
        return False, None, None, None
    if user.refresh_token != refresh_token :
        return False, None, None, None
    user.renew_session()
    db.session.commit()
    return True, user.session_token, user.session_expiration, user.refresh_token



#Returns (True, session_token, session_expiration, refresh_token) if successful, (False, None, None, None) if unsuccessful
def verify_credentials(username, passhash) :
    user = get_user_by_username(username)
    if (user is None) :
        return False, None, None, None
    if (user.verify_password(passhash)) :
        user.renew_session()
        return True, user.session_token, user.session_expiration, user.refresh_token

#creates new user, returns tokens
def create_user(email, username, passhash, salt, first_name, middle_initial, last_name, dob, role) :
    if role == "instructor" :
        user = Instructor(
            email=email,
            username=username,
            passhash=passhash,
            salt=salt,
            first_name=first_name,
            middle_initial=middle_initial,
            last_name=last_name,
            dob=dob,
        )
    elif role == "Tutor" :
        user = Tutor(
            email=email,
            username=username,
            passhash=passhash,
            salt=salt,
            first_name=first_name,
            middle_initial=middle_initial,
            last_name=last_name,
            dob=dob,
        )
    elif role == "student" :
        user = Student(
            email=email,
            username=username,
            passhash=passhash,
            salt=salt,
            first_name=first_name,
            middle_initial=middle_initial,
            last_name=last_name,
            dob=dob,
        )
    elif role == "admin" :
        user = Administrator(
            email=email,
            username=username,
            passhash=passhash,
            salt=salt,
            first_name=first_name,
            middle_initial=middle_initial,
            last_name=last_name,
            dob=dob,
        )
    else :
        user = User(
            email=email,
            username=username,
            passhash=passhash,
            salt=salt,
            first_name=first_name,
            middle_initial=middle_initial,
            last_name=last_name,
            dob=dob,
        )
    user.renew_session()
    db.session.add(user)
    db.session.commit()
    return user.session_token, user.session_expiration, user.refresh_token

