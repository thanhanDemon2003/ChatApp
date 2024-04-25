using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Chat.Domain.Entities;
using Chat.Infrastructure.Data;
using System.Security.Claims;
using ChatApp.Web.Hubs;
using Microsoft.AspNetCore.SignalR;
using ChatApp.Web.ViewModels;
using Chat.Application.Service.Interface;
using Microsoft.AspNetCore.Identity;
using ChatApp.Web.Models;
using System.Net;
using AutoMapper;
using ChatApp.Web.Models.DTOs;

namespace ChatApp.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiChatsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IHomeService _homeService;
        private readonly IMapper _mapper;
        protected APIResponse _response;


        public ApiChatsController(ApplicationDbContext db, IHubContext<ChatHub> hubContext, IHomeService homeService, IMapper mapper)
        {
            _db = db;
            _hubContext = hubContext;
            _homeService = homeService;
            _mapper = mapper;
            _response = new APIResponse();
        }
        [HttpPost]
        [Route("[controller]/LoginMobile")]
        public async Task<ActionResult<APIResponse>> LoginMobile(LoginVM model)
        {
            try
            {
                var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == model.Email);
                if (user == null)
                {
                    _response.Notification = new List<string> { "User không tồn tại" };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return _response;
                }
                var result = await _homeService.Login(model.Email, model.Password, false);
                if (result.Succeeded)
                {
                    var userData = await _db.Users.FirstOrDefaultAsync(x => x.Email == model.Email);
                    _response.Result = userData;
                    _response.Notification = new List<string> { "Đăng nhập thành công" };
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.IsSuccess = true;
                    return _response;
                }
                else
                {
                    _response.Notification = new List<string> { "Đăng nhập thất bại" };
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return _response;
                }
            }
            catch (Exception ex)
            {
                _response.Notification = new List<string> { ex.Message };
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                return _response;
            }
        }
        [HttpPost]
        [Route("[controller]/RegisterMobile")]
        public async Task<ActionResult<APIResponse>> RegisterMobile(RegisterVM model)
        {
            try
            {
                var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == model.Email);
                if (user != null)
                {
                    _response.Notification = new List<string> { "Email đã tồn tại" };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return _response;
                }
                var result = await _homeService.Register(model.Email, model.Name, model.Password);
                if (result.Succeeded)
                {
                    var userData = await _db.Users.FirstOrDefaultAsync(x => x.Email == model.Email);
                    _response.Result = userData;
                    _response.Notification = new List<string> { "Đăng ký thành công" };
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.IsSuccess = true;
                    return _response;
                }
                else
                {
                    _response.Notification = new List<string> { "Đăng ký thất bại" };
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return _response;
                }
            }
            catch (Exception ex)
            {
                _response.Notification = new List<string> { ex.Message };
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                return _response;
            }
        }
        [HttpGet]
        [Route("[controller]/GetAllGroupChat")]
        public async Task<ActionResult<APIResponse>> GetAllGroupChat()
        {
            try
            {
                var groupChats = await _db.Groups.Include(g => g.UserGroups).ThenInclude(ug => ug.User).ToListAsync();
                var groupChatDtos = new List<GroupDTO>();

                foreach (var groupChat in groupChats)
                {
                    var userGroupDtos = new List<UserDTO>();
                    foreach (var userGroup in groupChat.UserGroups)
                    {
                        var userGroupDto = _mapper.Map<UserDTO>(userGroup);
                        userGroupDtos.Add(userGroupDto);
                    }

                    var groupChatDto = new GroupDTO
                    {
                        GroupName = groupChat.GroupName,
                        CreatedDate = groupChat.CreatedDate,
                        CreatedBy = groupChat.CreatedBy,
                        UserGroups = userGroupDtos
                    };

                    groupChatDtos.Add(groupChatDto);
                }

                _response.Result = groupChatDtos;
                _response.Notification = new List<string> { "Lấy dữ liệu thành công" };
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                return _response;
            }
            catch (Exception ex)
            {
                _response.Notification = new List<string> { ex.Message };
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                return _response;
            }
        }
        [HttpGet]
        [Route("[controller]/GetAllUser")]
        public async Task<ActionResult<APIResponse>> GetAllUser()
        {
            try
            {
                var users = await _db.Users.GroupBy(x => x.Id).Select(x => new UserDTO
                {
                    Id = x.Key,
                    Email = x.FirstOrDefault().Email,
                    FullName = x.FirstOrDefault().Name,
                    ImageUrl = x.FirstOrDefault().ImageUrl,
                    PhoneNumber = x.FirstOrDefault().PhoneNumber,
                    Address = x.FirstOrDefault().Address
                }).ToListAsync();

                _response.Result = users;
                _response.Notification = new List<string> { "Lấy dữ liệu thành công" };
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                return _response;
            }
            catch (Exception ex)
            {
                _response.Notification = new List<string> { ex.Message };
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                return _response;
            }
        }
        [HttpPost]
        [Route("[controller]/CreateGroupChat")]
        public async Task<ActionResult<APIResponse>> CreateGroup([FromBody] RoomChatDTO roomChat)
        {
            try
            {
                var groupChat = new Group
                {
                    GroupName = roomChat.RoomName,
                    CreatedDate = DateTime.Now,
                    CreatedBy = roomChat.userCrate,
                    UserGroups = new List<UserGroup>()
                };
                var userCreateGroup = new UserGroup
                {
                    UserId = roomChat.userCrate,
                    GroupId = groupChat.GroupId,
                };
                groupChat.UserGroups.Add(userCreateGroup);
                foreach (var item in roomChat.userIds)
                {
                    var userGroup = new UserGroup
                    {
                        UserId = item,
                        GroupId = groupChat.GroupId
                    };
                    groupChat.UserGroups.Add(userGroup);
                }

                await _db.Groups.AddAsync(groupChat);
                await _db.SaveChangesAsync();

                foreach (var userConversation in groupChat.UserGroups)
                {
                    Conversation conversation = new Conversation
                    {
                        UserRefId = userConversation.UserId,
                        GroupId = groupChat.GroupId,
                        LatestMessageDateTime = DateTime.Now
                    };
                    await _db.Conversations.AddAsync(conversation);
                }
                await _db.SaveChangesAsync();
                var userGroupDtos = new List<UserDTO>();
                foreach (var userGroup in groupChat.UserGroups)
                {
                    await _db.Entry(userGroup).Reference(ug => ug.User).LoadAsync();
                    var userGroupDto = _mapper.Map<UserDTO>(userGroup);
                    userGroupDtos.Add(userGroupDto);
                }

                var groupChatDto = new GroupDTO
                {
                    GroupName = groupChat.GroupName,
                    CreatedDate = groupChat.CreatedDate,
                    CreatedBy = groupChat.CreatedBy,
                    UserGroups = userGroupDtos
                };

                _response.Result = groupChatDto;
                _response.Notification = new List<string> { "Tạo nhóm thành công" };
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                return _response;
            }
            catch (Exception ex)
            {
                _response.Notification = new List<string> { ex.Message };
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                return _response;
            }
        }
        [HttpPost]
        [Route("[controller]/SendMessageToGroup")]
        public async Task<ActionResult<APIResponse>> SendMessageToGroup([FromBody] MessageGroupDTO message)
        {
            try
            {
                var group = await _db.Groups.Include(g => g.UserGroups).ThenInclude(ug => ug.User).FirstOrDefaultAsync(x => x.GroupId == message.GroupId);
                if (group == null)
                {
                    _response.Notification = new List<string> { "Nhóm không tồn tại" };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return _response;
                }
                var userGroup = group.UserGroups.FirstOrDefault(x => x.UserId == message.senderId);
                if (userGroup == null)
                {
                    _response.Notification = new List<string> { "Bạn không thuộc nhóm này" };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return _response;
                }
                var messageEntity = new Message
                {
                    GroupId = message.GroupId,
                    UserId = message.senderId,
                    Content = message.Content,
                    SentDate = DateTime.Now
                };
                await _db.Messages.AddAsync(messageEntity);
                await _db.SaveChangesAsync();

                var messageDto = new MessageGroupDTO
                {
                    GroupId = messageEntity.GroupId,
                    senderId = messageEntity.UserId,
                    Content = messageEntity.Content,
                    CreatedDate = messageEntity.SentDate
                };


                //await _hubContext.Clients.Group(message.GroupId.ToString()).SendAsync("ReceiveMessage", messageDto);
                _response.Result = messageDto;
                _response.Notification = new List<string> { "Gửi tin nhắn thành công" };
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                return _response;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _response.Notification = new List<string> { ex.Message };
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                return _response;
            }
        }
        [HttpGet]
        [Route("[controller]/GetMessageByGroup")]
        public async Task<ActionResult<APIResponse>> GetMessageByGroup(int groupId)
        {
            try
            {
                var messages = await _db.Messages.Where(x => x.GroupId == groupId && x.isDeleted == false).ToListAsync();
                var messageDtos = new List<MessageGroupDTO>();
                foreach (var message in messages)
                {
                    var messageDto = new MessageGroupDTO
                    {
                        GroupId = message.GroupId,
                        senderId = message.UserId,
                        Content = message.Content,
                        CreatedDate = message.SentDate
                    };
                    messageDtos.Add(messageDto);
                }

                _response.Result = messageDtos;
                _response.Notification = new List<string> { "Lấy dữ liệu thành công" };
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                return _response;
            }
            catch (Exception ex)
            {
                _response.Notification = new List<string> { ex.Message };
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                return _response;
            }
        }

        [HttpPost]
        [Route("[controller]/SendMessageToUser")]
        public async Task<ActionResult<APIResponse>> SendMessageToUser([FromBody] MessageUserDTO message)
        {
            try
            {

                var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == message.senderId);
                if (user == null)
                {
                    _response.Notification = new List<string> { "Người dùng không tồn tại" };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return _response;
                }
                var receiver = await _db.Users.FirstOrDefaultAsync(x => x.Id == message.ReceiverId);
                if (receiver == null)
                {
                    _response.Notification = new List<string> { "Người nhận không tồn tại" };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return _response;
                }
                var conversation = await _db.Conversations.FirstOrDefaultAsync(x => x.UserRefId == message.ReceiverId && x.GroupId == null);
                if (conversation == null)
                {
                    conversation = new Conversation
                    {
                        UserRefId = message.senderId,
                        ReceiverId = message.ReceiverId,
                        LatestMessageDateTime = DateTime.Now
                    };
                    conversation = new Conversation
                    {
                        UserRefId = message.ReceiverId,
                        ReceiverId = message.senderId,
                        LatestMessageDateTime = DateTime.Now
                    };
                    await _db.Conversations.AddAsync(conversation);
                    await _db.SaveChangesAsync();
                }
                var messageEntity = new Message
                {
                    UserId = message.senderId,
                    ReceiverId = message.ReceiverId,
                    Content = message.Content,
                    SentDate = DateTime.Now
                };
                await _db.Messages.AddAsync(messageEntity);
                await _db.SaveChangesAsync();

                var messageDto = new MessageUserDTO
                {
                    senderId = messageEntity.UserId,
                    ReceiverId = messageEntity.ReceiverId,
                    Content = messageEntity.Content,
                    CreatedDate = messageEntity.SentDate
                };

                //await _hubContext.Clients.User(message.receiverId).SendAsync("ReceiveMessage", messageDto);
                _response.Result = messageDto;
                _response.Notification = new List<string> { "Gửi tin nhắn thành công" };
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                return _response;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _response.Notification = new List<string> { ex.Message };
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                return _response;
            }
        }

        [HttpGet]
        [Route("[controller]/GetMessageByUser")]
        public async Task<ActionResult<APIResponse>> GetMessageByUser(string senderId, string receiverId)
        {
            try
            {
                var messages = await _db.Messages.Where(x => x.UserId == senderId && x.ReceiverId == receiverId && x.isDeleted == false).ToListAsync();
                var messageDtos = new List<MessageUserDTO>();
                foreach (var message in messages)
                {

                    var messageDto = new MessageUserDTO
                    {
                        senderId = message.UserId,
                        ReceiverId = message.ReceiverId,
                        Content = message.Content,
                        CreatedDate = message.SentDate
                    };
                    messageDtos.Add(messageDto);
                }

                _response.Result = messageDtos;
                _response.Notification = new List<string> { "Lấy dữ liệu thành công" };
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                return _response;
            }
            catch (Exception ex)
            {
                _response.Notification = new List<string> { ex.Message };
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                return _response;
            }
        }

        [HttpGet]
        [Route("[controller]/GetAllMessage")]
        public async Task<ActionResult<APIResponse>> GetAllMessage()
        {
            try
            {
                var messages = await _db.Messages.ToListAsync();
                //foreach (var message in messages)
                //{
                //    await _db.Entry(message).Reference(m => m.User).LoadAsync();
                //    await _db.Entry(message).Reference(m => m.Receiver).LoadAsync();
                //    await _db.Entry(message).Reference(m => m.Group).LoadAsync();
                //}


                _response.Result = messages;
                _response.Notification = new List<string> { "Lấy dữ liệu thành công" };
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                return _response;
            }
            catch (Exception ex)
            {
                _response.Notification = new List<string> { ex.Message };
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                return _response;
            }
        }
        [HttpGet]
        [Route("[controller]/GetAllAttachment")]
        public async Task<ActionResult<APIResponse>> GetAllAttachment()
        {
            try
            {
                var attachments = await _db.Files.ToListAsync();
                _response.Result = attachments;
                _response.Notification = new List<string> { "Lấy dữ liệu thành công" };
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                return _response;
            }
            catch (Exception ex)
            {
                _response.Notification = new List<string> { ex.Message };
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                return _response;
            }
        }
        [HttpGet]
        [Route("[controller]/GetAttachmentByGroup")]
        public async Task<ActionResult<APIResponse>> GetAttachmentByGroup(int groupId)
        {
            try
            {
                var attachments = await _db.Files.Where(x => x.GroupId == groupId).ToListAsync();
                _response.Result = attachments;
                _response.Notification = new List<string> { "Lấy dữ liệu thành công" };
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                return _response;
            }
            catch (Exception ex)
            {
                _response.Notification = new List<string> { ex.Message };
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                return _response;
            }
        }
        [HttpGet]
        [Route("[controller]/GetAttachmentByUser")]
        public async Task<ActionResult<APIResponse>> GetAttachmentByUser(string senderId, string receiverId)
        {
            try
            {
                var attachments = await _db.Files.Where(x => x.UserId == senderId && x.ReceiverId == receiverId).ToListAsync();
                _response.Result = attachments;
                _response.Notification = new List<string> { "Lấy dữ liệu thành công" };
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                return _response;
            }
            catch (Exception ex)
            {
                _response.Notification = new List<string> { ex.Message };
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                return _response;
            }
        }

        [HttpGet]
        [Route("[controller]/GetChatYour/{idUser}")]
        public async Task<ActionResult<APIResponse>> GetChatYour(string idUser)
        {
            try
            {
                var messUser = await _db.Messages.Where(x => x.UserId == idUser).ToListAsync();
                var messReceiver = await _db.Messages.Where(x => x.ReceiverId == idUser).ToListAsync();
                var messGroup = await _db.Messages.Where(x => x.GroupId != null && x.isDeleted == false).ToListAsync();
                var messUserGroup = new List<Message>();
                foreach (var item in messGroup)
                {
                    var userGroup = await _db.UserGroups.FirstOrDefaultAsync(x => x.GroupId == item.GroupId && x.UserId == idUser);
                    if (userGroup != null)
                    {
                        messUserGroup.Add(item);
                    }
                }
                var mess = messUser.Concat(messReceiver).Concat(messUserGroup).ToList();
                _response.Result = mess;
                _response.Notification = new List<string> { "Lấy dữ liệu thành công" };
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                return _response;
            }
            catch (Exception ex)
            {
                _response.Notification = new List<string> { ex.Message };
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                return _response;
            }
        }


        [HttpDelete]
        [Route("[controller]/DeleteGroup/{idGroup}")]
        public async Task<ActionResult<APIResponse>> DeleteGroup(int idGroup, string createBy)
        {
            try
            {

                var group = await _db.Groups.FirstOrDefaultAsync(x => x.GroupId == idGroup);
                if (group == null)
                {
                    _response.Notification = new List<string> { "Nhóm không tồn tại" };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return _response;
                }
                if (group.CreatedBy != createBy)
                {
                    _response.Notification = new List<string> { "Bạn không phải là người tạo nhóm" };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return _response;
                }
                group.isDeleted = true;
                var messages = await _db.Messages.Where(x => x.GroupId == idGroup).ToListAsync();
                foreach (var message in messages)
                {
                    message.isDeleted = true;
                }
                _db.Messages.UpdateRange(messages);
                _db.Groups.Update(group);
                await _db.SaveChangesAsync();
                _response.Notification = new List<string> { "Xóa nhóm thành công" };
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                return _response;
            }
            catch (Exception ex)
            {
                _response.Notification = new List<string> { ex.Message };
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                return _response;
            }
        }
        [HttpDelete]
        [Route("[controller]/DeleteMessage/{idMessage}")]
        public async Task<ActionResult<APIResponse>> DeleteMessage(int idMessage, string senderId)
        {
            try
            {
                var message = await _db.Messages.FirstOrDefaultAsync(x => x.MessageId == idMessage);
                if (message == null)
                {
                    _response.Notification = new List<string> { "Tin nhắn không tồn tại" };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                if (message.UserId != senderId)
                {
                    _response.Notification = new List<string> { "Bạn không phải là người gửi tin nhắn" };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                message.isDeleted = true;
                _db.Messages.Update(message);
                await _db.SaveChangesAsync();
                _response.Notification = new List<string> { "Xóa tin nhắn thành công" };
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Notification = new List<string> { ex.Message };
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                return _response;
            }
        }

        [HttpDelete]
        [Route("[controller]/DeleteAttachment/{idAttachment}")]
        public async Task<ActionResult<APIResponse>> DeleteAttachment(int idAttachment, string senderId)
        {
            try
            {
                var attachment = await _db.Files.FirstOrDefaultAsync(x => x.AttachmentId == idAttachment);
                if (attachment == null)
                {
                    _response.Notification = new List<string> { "File không tồn tại" };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                if (attachment.UserId != senderId)
                {
                    _response.Notification = new List<string> { "Bạn không phải là người gửi file" };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                _db.Files.Remove(attachment);
                await _db.SaveChangesAsync();
                _response.Notification = new List<string> { "Xóa file thành công" };
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.Notification = new List<string> { ex.Message };
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                return _response;
            }
        }

        [HttpDelete]
        [Route("[controller]/DeleteUserInGroup/{idGroup}")]
        public async Task<ActionResult<APIResponse>> DeleteUserInGroup(int idGroup, string idUser, string createBy)
        {
            try
            {
                var group = await _db.Groups.Include(g => g.UserGroups).FirstOrDefaultAsync(x => x.GroupId == idGroup);
                if (group == null)
                {
                    _response.Notification = new List<string> { "Nhóm không tồn tại" };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return _response;
                }
                if (group.CreatedBy != createBy)
                {
                    _response.Notification = new List<string> { "Bạn không phải là người tạo nhóm" };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return _response;
                }
                var userGroup = group.UserGroups.FirstOrDefault(x => x.UserId == idUser);
                if (userGroup == null)
                {
                    _response.Notification = new List<string> { "Người dùng không thuộc nhóm" };
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return _response;
                }
                group.UserGroups.Remove(userGroup);
                _db.Groups.Update(group);
                await _db.SaveChangesAsync();
                _response.Notification = new List<string> { "Xóa người dùng khỏi nhóm thành công" };
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                return _response;
            }
            catch (Exception ex)
            {
                _response.Notification = new List<string> { ex.Message };
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                return _response;
            }
        }
        [HttpDelete]
        [Route("[controller]/DeleteMessageChatUser")]
        public async Task<ActionResult<APIResponse>> DeleteMessageChatUser(string senderId, string receiverId)
        {
            try
            {
                var messages = await _db.Messages.Where(x => x.UserId == senderId && x.ReceiverId == receiverId).ToListAsync();
                foreach (var message in messages)
                {
                    message.isDeleted = true;
                }
                _db.Messages.UpdateRange(messages);
                await _db.SaveChangesAsync();
                _response.Notification = new List<string> { "Xóa tin nhắn thành công" };
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                return _response;
            }
            catch (Exception ex)
            {
                _response.Notification = new List<string> { ex.Message };
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                return _response;
            }
        }

    }
}
