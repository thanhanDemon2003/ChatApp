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
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IHomeService _homeService;
        private readonly IMapper _mapper;
        protected APIResponse _response;
        public ApiChatsController(ApplicationDbContext context,
           IHubContext<ChatHub> hubContext
            , IHomeService homeService,
           IMapper mapper
            )
        {
            _context = context;
            _hubContext = hubContext;
            _homeService = homeService;
            _mapper = mapper;
        }

        //GET: api/ChatRooms
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("/[controller]/GetChatRoom")]
        public async Task<ActionResult<APIResponse>> GetChatRoom()
        {
            try
            {
                IEnumerable<Conversation> conversations;
                conversations = await _context.Conversations.ToListAsync();
                return new APIResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    IsSuccess = true,
                    Result = conversations
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message }
                };
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("/[controller]/GetChatUser/{userId}")]
        public async Task<ActionResult<APIResponse>> GetChatUser(string userId)
        {
            try
            {
                //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return new APIResponse
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        IsSuccess = false,
                        ErrorMessages = new List<string> { "userId không tồn tại!" }
                    };
                }
                var users = await _context.Users.ToListAsync();
                var res = users.Where(u => u.Id != userId).Select(u => new { u.Id, u.UserName }).ToList();
                return new APIResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Result = res
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message }
                };
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("/[controller]/GetChatRoomUser/{userId}")]
        public async Task<ActionResult<APIResponse>> GetChatRoomFromUser(string userId)
        {
            try
            {
                IEnumerable<UserConversation> userConversations;
                userConversations = await _context.UserConversations
                      .Where(uc => uc.UserRefId == userId)
                      .Join(_context.Conversations,
                            uc => uc.ConversationId,
                            c => c.ConversationId,
                            (uc, c) => new { UserConversation = uc, Conversation = c })
                      .OrderByDescending(x => x.Conversation.UpdatedAt)
                      .Select(x => x.UserConversation)
                      .ToListAsync();
                var conversations = new List<Conversation>();

                foreach (var uc in userConversations)
                {
                    var conversation = await _context.Conversations.FindAsync(uc.ConversationId);
                    conversations.Add(conversation);
                }
                return new APIResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Result = conversations
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message }
                };
            }
        }
        //("{roomName}")
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("/[controller]/CreateRoomChat")]
        public async Task<ActionResult<APIResponse>> CreateRoomChat([FromBody] RoomChatDTO roomChat)
        {
            try
            {
                if (roomChat == null)
                {
                    return new APIResponse
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        IsSuccess = false,
                        ErrorMessages = new List<string> { "Dữ liệu không hợp lệ" }
                    };
                }
                var userId = roomChat.userCrate;
                var conversation = new Conversation
                {
                    Name = roomChat.RoomName,
                    ConversationType = ConversationType.Group,
                    CreatedAt = DateTime.Now,
                    UserConversations = new List<UserConversation>
                {
                    new UserConversation
                    {
                        UserRefId = userId
                    }
                }
                };

                foreach (var id in roomChat.userIds)
                {
                    conversation.UserConversations.Add(new UserConversation
                    {
                        UserRefId = id
                    });
                }

                _context.Conversations.Add(conversation);
                await _context.SaveChangesAsync();

                return new APIResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Result = conversation
                };
            }
            catch
            {
                return new APIResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { "Lỗi khi tạo phòng chat" }
                };
            }
        }
        // ("{id}")
        [HttpDelete]
        [Route("/[controller]/DeleteRoomChat/{id}")]
        public async Task<ActionResult<APIResponse>> DeleteRoomChat(int id)
        {
            try
            {
                var chatRoom = await _context.Conversations.FindAsync(id);
                if (chatRoom == null)
                {
                    return NotFound();
                }

                _context.Conversations.Remove(chatRoom);
                await _context.SaveChangesAsync();

                var room = await _context.Conversations.FirstOrDefaultAsync();

                return new APIResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Result = new { deleted = id, selected = (room == null ? 0 : room.ConversationId) }
                };
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message };
                return CreatedAtRoute("DeleteRoomChat", new { id = id }, _response);
            }
        }
        //POST: api/ChatRooms
        //To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //[Route("/[controller]/PostChatRoom")]
        //public async Task<ActionResult<Conversation>> PostChatRoom(Conversation chatRoom)
        //{
        //    if (_context.Conversations == null)
        //    {
        //        return Problem("Entity set 'ApplicationDbContext.ChatRoom'  is null.");
        //    }
        //    _context.Conversations.Add(chatRoom);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetChatRoom", new { id = chatRoom.ConversationId }, chatRoom);
        //}
        [HttpPost]
        [Route("/[controller]/PostMessageConversationGroup")]
        public async Task<ActionResult<APIResponse>> PostMessageGroup(MessagesDTO mess)
        {
            try
            {
                var cv = await _context.Conversations.FindAsync(mess.ConversationId);
                if (cv == null)
                {
                    return new APIResponse
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        IsSuccess = false,
                        ErrorMessages = new List<string> { "Không tìm thấy cuộc trò chuyện" }
                    };
                }
                else
                {
                    if (mess.AttachmentUrl != null)
                    {
                        Attachment attachment = new()
                        {
                            conversationId = mess.ConversationId,
                            senderId = mess.senderId,
                            FileUrl = mess.AttachmentUrl,
                            FileType = mess.AttachmentType,
                            UploadedAt = DateTime.Now

                        };
                        cv.Attachments.Add(attachment);
                    }
                    else
                    {
                        Message message = new()
                        {
                            ConversationId = mess.ConversationId,
                            UserRefId = mess.senderId,
                            Content = mess.Content,
                            SentAt = DateTime.Now
                        };
                        cv.Messages.Add(message);
                    }
                    cv.UpdatedAt = DateTime.Now;
                    _context.Conversations.Update(cv);
                    await _context.SaveChangesAsync();
                    _ = _hubContext.Clients.Group(cv.ConversationId.ToString()).SendAsync("ReceiveMessage", mess);
                    return new APIResponse { StatusCode = HttpStatusCode.OK, IsSuccess = true, Result = mess };
                }
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message }
                };
            }

        }
        //("{receiverId}")
        [HttpPost]
        [Route("/[controller]/PostPrivateChat")]
        public async Task<ActionResult<APIResponse>> PostPrivateChat(MessagesDTO mess)
        {
            try
            {
                var conversation = await _context.Conversations
                    .Where(c => c.ConversationType == ConversationType.Private)
                    .Include(c => c.UserConversations)
                    .FirstOrDefaultAsync(c => c.UserConversations.Any(uc => uc.UserRefId == mess.senderId));
                if (conversation == null)
                {
                    conversation = new Conversation
                    {
                        ConversationType = ConversationType.Private,
                        CreatedAt = DateTime.Now,
                        UserConversations = new List<UserConversation>
                        {
                            new UserConversation
                            {
                                UserRefId = mess.senderId
                            },
                            new UserConversation
                            {
                                UserRefId = mess.senderId
                            }
                        }
                    };
                    _context.Conversations.Add(conversation);
                    await _context.SaveChangesAsync();
                }
                if (mess.AttachmentUrl != null)
                {
                    Attachment attachment = new()
                    {
                        conversationId = conversation.ConversationId,
                        senderId = mess.senderId,
                        FileUrl = mess.AttachmentUrl,
                        FileType = mess.AttachmentType,
                        UploadedAt = DateTime.Now

                    };
                    conversation.Attachments.Add(attachment);
                    _context.Conversations.Update(conversation);
                    await _context.SaveChangesAsync();
                    _ = _hubContext.Clients.Group(conversation.ConversationId.ToString()).SendAsync("ReceiveMessage", attachment);
                    return new APIResponse { StatusCode = HttpStatusCode.OK, IsSuccess = true, Result = attachment };
                }
                else
                {
                    Message message = new()
                    {
                        ConversationId = conversation.ConversationId,
                        UserRefId = mess.senderId,
                        Content = mess.Content,
                        SentAt = DateTime.Now
                    };
                    conversation.Messages.Add(message);
                    _context.Conversations.Update(conversation);
                    await _context.SaveChangesAsync();
                    _ = _hubContext.Clients.Group(conversation.ConversationId.ToString()).SendAsync("ReceiveMessage", message);
                    return new APIResponse { StatusCode = HttpStatusCode.OK, IsSuccess = true, Result = message };

                }


            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message }
                };
            }
        }
        //("{idConversation}")
        [HttpGet]
        [Route("/[controller]/GetMessageConversation")]
        public async Task<ActionResult<APIResponse>> GetMessageGroup(int idConversation)
        {
            try
            {
                if (idConversation == null)
                {
                    return new APIResponse
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        IsSuccess = false,
                        ErrorMessages = new List<string> { "idConversation không tồn tại!" }
                    };
                }
                var messages = await _context.Conversations.Where(m => m.ConversationId == idConversation).
                    SelectMany(m => m.Messages).OrderBy(m => m.SentAt).ToListAsync();
                return new APIResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Result = messages
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message }
                };
            }

        }
        //("{name}")
        [HttpPost]
        [Route("/[controller]/SearchUser")]
        public async Task<ActionResult<APIResponse>> SearchUser(string name)
        {
            try
            {
                var users = await _context.Users.Where(u => u.Name.Contains(name)).ToListAsync();
                return new APIResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Result = users
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message }
                };
            }
        }
        [HttpPost]
        [Route("/[controller]/LoginMobile")]
        public async Task<ActionResult> LoginMobile(LoginVM login)
        {
            var result = await _homeService.Login(login.Email, login.Password, login.RememberMe);

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    return Ok(new ResVM<LoginVM>
                    {
                        data = null,
                        Message = "Tài khoản đã bị khóa",
                        Success = false
                    });
                }
                else
                {
                    return Ok(new ResVM<LoginVM>
                    {
                        data = null,
                        Message = "Mật khẩu không chính xác",
                        Success = false
                    });
                }
            }

            // Lấy thông tin user từ kết quả đăng nhập thành công
            var user = await _homeService.GetUserToEmail(login.Email);

            return Ok(new ResVM<ApplicationUser>
            {
                data = user,
                Message = "Đăng nhập thành công",
                Success = true
            });
        }
        [HttpPost]
        [Route("/[controller]/RegisterMobile")]
        public async Task<ActionResult> RegisterMobile(RegisterVM register)
        {
            var result = await _homeService.Register(register.Email, register.Name, register.Password);

            if (!result.Succeeded)
            {
                return Ok(new ResVM<RegisterVM>
                {
                    data = null,
                    Message = "Đăng ký không thành công",
                    Success = false
                });
            }

            // Lấy thông tin user từ kết quả đăng ký thành công
            var user = await _homeService.GetUserToEmail(register.Email);

            return Ok(new ResVM<ApplicationUser>
            {
                data = user,
                Message = "Đăng ký thành công",
                Success = true
            });
        }

        //[HttpPost]
        //[Route("/[controller]/PostMessagePrivate")]
        //public async Task<ActionResult> PostMessagePrivate(Message message)
        //{
        //    if (_context.Messages == null)
        //    {
        //        return Problem("Entity set 'ApplicationDbContext.Message'  is null.");
        //    }
        //    _context.Messages.Add(message);
        //    await _context.SaveChangesAsync();

        //    await _hubContext.Clients.User(message.Sender.Id).SendAsync("ReceiveMessage", message);

        //    return CreatedAtAction("GetChatRoom", new { id = message.MessageId }, message);
        //}



        //[HttpGet("{idConversation}")]
        //[Route("/[controller]/GetMessagePrivate")]
        //public async Task<ActionResult<IEnumerable<Message>>> GetMessagePrivate(int idConversation)
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var messages = await _context.Messages.Where(m => m.ConversationId == idConversation && m.Sender.Id == userId).ToListAsync();

        //    if (messages == null)
        //    {
        //        return NotFound();
        //    }

        //    return messages;
        //}


        ////// DELETE: api/ChatRooms/5
        ////[HttpDelete("{id}")]
        ////[Route("/[controller]/DeleteChatRoom/{id}")]
        ////public async Task<IActionResult> DeleteChatRoom(int id)
        ////{
        ////    if (_context.Conversations == null)
        ////    {
        ////        return NotFound();
        ////    }
        ////    var chatRoom = await _context.Conversations.FindAsync(id);
        ////    if (chatRoom == null)
        ////    {
        ////        return NotFound();
        ////    }

        ////    _context.Conversations.Remove(chatRoom);
        ////    await _context.SaveChangesAsync();

        ////    var room = await _context.Conversations.FirstOrDefaultAsync();

        ////    return Ok(new { deleted = id, selected = (room == null ? 0 : room.ConversationId) });
        ////}

        //[HttpPost]
        //[Route("/[controller]/GetChatPrivate")]
        //public async Task<ActionResult<IEnumerable<Message>>> GetChatPrivate([FromBody] int id)
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var messages = await _context.Messages.Where(m => m.ConversationId == id && m.Sender.Id == userId).ToListAsync();

        //    if (messages == null)
        //    {
        //        return NotFound();
        //    }

        //    return messages;
        //}


    }
}
