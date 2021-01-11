from db import User, Conversation, Message, db
from datetime import datetime
import users_dao

def get_conversation_by_id(id) :
    return Conversation.query.filter_by(id=id).first()

def create_conversation(user_1_id, user_2_id) :
    convo = Conversation(user_1_id=user_1_id, user_2_id=user_2_id)
    db.session.add(convo)
    db.session.commit()
    return convo

def create_message(sender_id, recipient_id, body, conversation_id) :
    time_sent = datetime.now()
    message = Message(sender_id=sender_id, recipient_id=recipient_id, body=body, conversation_id = conversation_id, time_sent=time_sent)
    db.session.add(message)
    db.session.commit()
    return message

#returns a boolean stating if the conversation exists, and the conversation (if it exists)
def check_conversation_exists(user_1_id, user_2_id) :
    user_1 = users_dao.get_user_by_id(user_1_id)
    conversations = user_1.conversations
    for convo in conversations :
        if (user_2_id == convo.user_1_id or user_2_id == convo.user_2_id) :
            return True, convo
    return False, None
