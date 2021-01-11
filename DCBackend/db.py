import datetime, hashlib, os

from flask_sqlalchemy import SQLAlchemy

db = SQLAlchemy()


class Class(db.Model):
    __tablename__ = "class"
    id = db.Column(db.Integer, primary_key=True)
    name = db.Column(db.String, nullable=False)
    users = db.relationship("User", back_populates="section")

    def serialize(self):
        users = [{"user" : assoc.external_serialize(),
                        "role" : assoc.role} for assoc in self.users]
        return {
            "id" : self.id,
            "name" : self.name,
            "users" : users
        }
    def external_serialize(self):
        return {
            "id" : self.id,
            "name" : self.name,
        }


class User(db.Model) :
    __tablename__ = "user"
    #User information
    id = db.Column(db.Integer, primary_key=True)
    email = db.Column(db.String, nullable=False, unique=True)
    username = db.Column(db.String, nullable=False, unique=True)
    passhash = db.Column(db.String, nullable=False)
    salt = db.Column(db.String, nullable=False)

    #Personal information
    first_name = db.Column(db.String, nullable=False)
    middle_initial = db.Column(db.String)
    last_name = db.Column(db.String, nullable=False)
    dob = db.Column(db.Date)
    #"administrator", "instructor", "tutor", or "student"
    role = db.Column(db.String, nullable=False)
    #Any other information, such as allergies
    notes = db.Column(db.String)

    #Relationship information
    class_id = db.Column(db.Integer, db.ForeignKey("class.id"))
    section = db.relationship("Class", back_populates="users", foreign_keys=[class_id])
    conversations = db.relationship("Conversation",
                                    primaryjoin="or_(User.id==Conversation.user_1_id, User.id==Conversation.user_2_id)",
                                    )
    #Session information
    session_token = db.Column(db.String, nullable=False, unique=True)
    session_expiration = db.Column(db.DateTime, nullable=False)
    refresh_token = db.Column(db.String, nullable=False, unique=True)

    #Inheritance information
    __mapper_args__ = {
        'polymorphic_on': role,
        'polymorphic_identity': 'user'
    }

    #gets the salt for password purposes
    def get_salt(self):
        return self.salt

    #randomly generate new token
    def _urlsafe_base_64_(self):
        return hashlib.sha1(os.urandom(64)).hexdigest()

    #renew session tokens
    def renew_session(self):
        self.session_token = self._urlsafe_base_64_()
        self.session_expiration = datetime.datetime.now() + datetime.timedelta(days=1)
        self.refresh_token = self._urlsafe_base_64_()

    #verifies that the given password is correct
    def verify_password(self, passhash):
        return self.passhash == passhash

    #verifies that the session has not expired and that the session token is correct
    def verify_session_token(self, session_token):
        return session_token == self.session_token and datetime.datetime.now() < self.session_expiration

    #verifies that the update token is correct
    def verify_refresh_token(self, refresh_token):
        return refresh_token == self.refresh_token



    def serialize(self):
            return {
                "id" : self.id,
                "first_name": self.first_name,
                "middle_initial": self.middle_initial,
                "last_name": self.last_name,
                "role" : self.role,
                "dob": self.dob.isoformat(),
                "section" : self.section.external_serialize(),
            }
    #serialize w/o courses
    def external_serialize(self):
        return {
            "id": self.id,
            "first_name": self.first_name,
            "middle_initial": self.middle_initial,
            "last_name": self.last_name,
            "dob": self.dob.isoformat(),
        }

class Student(User) :
    __tablename__ = "student"
    id = db.Column(db.Integer, db.ForeignKey("user.id"), primary_key=True)
    student_reports = db.relationship("Report", back_populates="student")
    tutor_id = db.Column(db.Integer, db.ForeignKey("student.id"))

    # Inheritance information
    __mapper_args__ = {
        'polymorphic_identity': 'student',
        'inherit_condition' : (id == User.id)
    }

    # returns the tutor's id, or none if user does not have a tutor
    def get_tutor(self):
        return self.tutor_id


class Tutor(User) :
    __tablename__ = "tutor"
    id = db.Column(db.Integer, db.ForeignKey("user.id"), primary_key=True)
    submitted_reports = db.relationship("Report", back_populates="tutor")
    students = db.relationship("Student")

    # Inheritance information
    __mapper_args__ = {
        'polymorphic_identity': 'tutor',
        'inherit_condition' : (id == User.id)
    }

    # returns a list of the students' ids
    def get_students(self):
        return [s.id for s in self.students]

class Instructor(User) :
    __tablename__ = "instructor"
    id = db.Column(db.Integer, db.ForeignKey("user.id"), primary_key=True)

    # Inheritance information
    __mapper_args__ = {
        'polymorphic_identity': 'instructor',
        'inherit_condition' : (id == User.id)
    }

class Administrator(User) :
    __tablename__ = "administrator"
    id = db.Column(db.Integer, db.ForeignKey("user.id"), primary_key=True)

    # Inheritance information
    __mapper_args__ = {
        'polymorphic_identity': 'admin',
        'inherit_condition': (id == User.id)
    }

class Report(db.Model) :
    __tablename__ = "report"
    id = db.Column(db.Integer, primary_key=True)
    tutor_id = db.Column(db.Integer, db.ForeignKey("tutor.id"), nullable=False)
    student_id = db.Column(db.Integer, db.ForeignKey("student.id"), nullable=False)
    tutor = db.relationship("Tutor", foreign_keys=[tutor_id], back_populates="submitted_reports")
    student = db.relationship("Student", foreign_keys=[student_id], back_populates="student_reports")
    report = db.Column(db.String, nullable=False)

    def serialize(self):
        return {
            "id": self.id,
            "tutor_id": self.tutor_id,
            "student_id": self.student_id,
            "report": self.report
        }

class Message(db.Model) :
    __tablename__ = "message"
    id = db.Column(db.Integer, primary_key=True)
    conversation_id = db.Column(db.Integer, db.ForeignKey("conversation.id"))
    conversation = db.relationship("Conversation", foreign_keys=[conversation_id], back_populates="messages")
    sender_id = db.Column(db.Integer, db.ForeignKey("user.id"), nullable=False)
    recipient_id = db.Column(db.Integer, db.ForeignKey("user.id"), nullable=False)
    body = db.Column(db.String, nullable=False)
    time_sent = db.Column(db.DateTime, nullable=False)

    def serialize(self):
        return {
            "id": self.id,
             "sender_id": self.sender_id,
             "recipient_id": self.recipient_id,
             "body": self.body,
             "time_sent": self.time_sent.isoformat()
        }

class Conversation(db.Model) :
    __tablename__ = "conversation"
    id = db.Column(db.Integer, primary_key=True)
    user_1_id = db.Column(db.Integer, db.ForeignKey("user.id"), nullable=False)
    user_2_id = db.Column(db.Integer, db.ForeignKey("user.id"), nullable=False)
    user_1 = db.relationship("User", foreign_keys=[user_1_id], backref="conversations_as_1")
    user_2 = db.relationship("User", foreign_keys=[user_2_id], backref="conversations_as_2")
    messages = db.relationship("Message", order_by="desc(Message.time_sent)", back_populates="conversation")
