using chatcommon.Classes;
using chatcommon.Entities;
using chatgateway.ChatRoomWebService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatgateway.Helper
{
    static class ServiceHelper
    {

        //====================================================================================
        //===============================[ User ]===========================================
        //====================================================================================

        public static List<User> ArrayTypeToUser(this UserChatRoom[] userChatRoomList)
        {
            object _lock = new object(); List<User> returnList = new List<User>();
            if (userChatRoomList != null)
            {
                //Parallel.ForEach(userChatRoomList, (userChat) =>
                foreach (var userChat in userChatRoomList)
                {
                    User user = new User();
                    user.ID = userChat.ID;
                    user.FirstName = Utility.decodeBase64ToString(userChat.FirstName);
                    user.LastName = Utility.decodeBase64ToString(userChat.LastName);
                    user.Username = Utility.decodeBase64ToString(userChat.Username);
                    user.Password = Utility.decodeBase64ToString(userChat.Password);
                    user.Email = Utility.decodeBase64ToString(userChat.Email);

                    lock (_lock) returnList.Add(user);
                }
                //);
            }
            return returnList;
        }

        public static UserChatRoom[] UserTypeToArray(this List<User> userList)
        {
            int i = 0;
            UserChatRoom[] returnChatArray = new UserChatRoom[userList.Count];
            if (userList != null)
            {
                Parallel.ForEach(userList, (user) =>
                {
                    UserChatRoom userChat = new UserChatRoom();
                    userChat.ID = user.ID;
                    userChat.FirstName = Utility.encodeStringToBase64(user.FirstName);
                    userChat.LastName = Utility.encodeStringToBase64(user.LastName);
                    userChat.Username = Utility.encodeStringToBase64(user.Username);
                    userChat.Password = Utility.encodeStringToBase64(user.Password);
                    userChat.Email = Utility.encodeStringToBase64(user.Email);

                    returnChatArray[i] = userChat;
                    i++;
                });
            }
            return returnChatArray;
        }

        public static UserFilterChatRoom UserTypeToFilterArray(this User user, string filterOperator)
        {
            UserFilterChatRoom userChat = new UserFilterChatRoom();
            if (user != null)
            {
                userChat.ID = user.ID;
                userChat.FirstName = Utility.encodeStringToBase64(user.FirstName);
                userChat.LastName = Utility.encodeStringToBase64(user.LastName);
                userChat.Username = Utility.encodeStringToBase64(user.Username);
                userChat.Password = Utility.encodeStringToBase64(user.Password);
                userChat.Email = Utility.encodeStringToBase64(user.Email);
                userChat.Operator = Utility.encodeStringToBase64(filterOperator);
            }
            return userChat;
        }

        //====================================================================================
        //===============================[ Discussion ]===========================================
        //====================================================================================

        public static List<Discussion> ArrayTypeToDiscussion(this DiscussionChatRoom[] discussionChatRoomList)
        {
            object _lock = new object(); List<Discussion> returnList = new List<Discussion>();
            if (discussionChatRoomList != null)
            {
                //Parallel.ForEach(discussionChatRoomList, (discussionChat) =>
                foreach (var discussionChat in discussionChatRoomList)
                {
                    Discussion discussion = new Discussion();
                    discussion.ID = discussionChat.ID;
                    discussion.MessageId = discussionChat.MessageId;
                    discussion.Date = Utility.convertToDateTime(Utility.decodeBase64ToString(discussionChat.Date));
                    
                    lock (_lock) returnList.Add(discussion);
                }
                //);
            }
            return returnList;
        }

        public static DiscussionChatRoom[] DiscussionTypeToArray(this List<Discussion> discussionList)
        {
            int i = 0;
            DiscussionChatRoom[] returnChatArray = new DiscussionChatRoom[discussionList.Count];
            if (discussionList != null)
            {
                Parallel.ForEach(discussionList, (discussion) =>
                {
                    DiscussionChatRoom discussionChat = new DiscussionChatRoom();
                    discussionChat.ID = discussion.ID;
                    discussionChat.MessageId = discussion.MessageId;
                    discussionChat.Date = Utility.encodeStringToBase64( discussion.Date.ToString());

                    returnChatArray[i] = discussionChat;
                    i++;
                });
            }
            return returnChatArray;
        }

        public static DiscussionFilterChatRoom DiscussionTypeToFilterArray(this Discussion discussion, string filterOperator)
        {
            DiscussionFilterChatRoom discussionChat = new DiscussionFilterChatRoom();
            if (discussion != null)
            {
                discussionChat.ID = discussion.ID;
                discussionChat.MessageId = discussion.MessageId;
                discussionChat.Date = Utility.encodeStringToBase64(discussion.Date.ToString());
                discussionChat.Operator = Utility.encodeStringToBase64(filterOperator);
            }
            return discussionChat;
        }

        //====================================================================================
        //===============================[ Message ]===========================================
        //====================================================================================

        public static List<Message> ArrayTypeToMessage(this MessageChatRoom[] messageChatRoomList)
        {
            object _lock = new object(); List<Message> returnList = new List<Message>();
            if (messageChatRoomList != null)
            {
                //Parallel.ForEach(messageChatRoomList, (messageChat) =>
                foreach (var messageChat in messageChatRoomList)
                {
                    Message message = new Message();
                    message.ID = messageChat.ID;
                    message.Date = Utility.convertToDateTime(Utility.decodeBase64ToString(messageChat.Date));
                    message.Content = Utility.decodeBase64ToString(messageChat.Content);
                    
                    lock (_lock) returnList.Add(message);
                }
                //);
            }
            return returnList;
        }

        public static MessageChatRoom[] MessageTypeToArray(this List<Message> messageList)
        {
            int i = 0;
            MessageChatRoom[] returnChatArray = new MessageChatRoom[messageList.Count];
            if (messageList != null)
            {
                Parallel.ForEach(messageList, (message) =>
                {
                    MessageChatRoom messageChat = new MessageChatRoom();
                    messageChat.ID = message.ID;
                    messageChat.Date = Utility.encodeStringToBase64(message.Date.ToString());
                    messageChat.Content = Utility.encodeStringToBase64(message.Content);

                    returnChatArray[i] = messageChat;
                    i++;
                });
            }
            return returnChatArray;
        }

        public static MessageFilterChatRoom MessageTypeToFilterArray(this Message message, string filterOperator)
        {
            MessageFilterChatRoom messageChat = new MessageFilterChatRoom();
            if (message != null)
            {
                messageChat.ID = message.ID;
                messageChat.Date = Utility.encodeStringToBase64(message.Date.ToString());
                messageChat.Content = Utility.encodeStringToBase64(message.Content);
                messageChat.Operator = Utility.encodeStringToBase64(filterOperator);
            }
            return messageChat;
        }

        //====================================================================================
        //===============================[ User_discussion ]===========================================
        //====================================================================================

        public static List<User_discussion> ArrayTypeToUser_discussion(this User_discussionChatRoom[] user_discussionChatRoomList)
        {
            object _lock = new object(); List<User_discussion> returnList = new List<User_discussion>();
            if (user_discussionChatRoomList != null)
            {
                //Parallel.ForEach(user_discussionChatRoomList, (user_discussionChat) =>
                foreach (var user_discussionChat in user_discussionChatRoomList)
                {
                    User_discussion user_discussion = new User_discussion();
                    user_discussion.ID = user_discussionChat.ID;
                    user_discussion.UserId = user_discussionChat.UserId;
                    user_discussion.DiscussionId = user_discussionChat.DiscussionId;
                    
                    lock (_lock) returnList.Add(user_discussion);
                }
                //);
            }
            return returnList;
        }

        public static User_discussionChatRoom[] User_discussionTypeToArray(this List<User_discussion> user_discussionList)
        {
            int i = 0;
            User_discussionChatRoom[] returnChatArray = new User_discussionChatRoom[user_discussionList.Count];
            if (user_discussionList != null)
            {
                Parallel.ForEach(user_discussionList, (user_discussion) =>
                {
                    User_discussionChatRoom user_discussionChat = new User_discussionChatRoom();
                    user_discussionChat.ID = user_discussion.ID;
                    user_discussionChat.UserId = user_discussion.UserId;
                    user_discussionChat.DiscussionId = user_discussion.DiscussionId;

                    returnChatArray[i] = user_discussionChat;
                    i++;
                });
            }
            return returnChatArray;
        }

        public static User_discussionFilterChatRoom User_discussionTypeToFilterArray(this User_discussion user_discussion, string filterOperator)
        {
            User_discussionFilterChatRoom user_discussionChat = new User_discussionFilterChatRoom();
            if (user_discussion != null)
            {
                user_discussionChat.ID = user_discussion.ID;
                user_discussionChat.UserId = user_discussion.UserId;
                user_discussionChat.DiscussionId = user_discussion.DiscussionId;
                user_discussionChat.Operator = Utility.encodeStringToBase64(filterOperator);
            }
            return user_discussionChat;
        }


    }
}
