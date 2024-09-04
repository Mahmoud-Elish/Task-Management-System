using Microsoft.AspNetCore.Http.HttpResults;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace TaskManagement.API;

public record CommentInfoDto
(
 int Id,
 string Content, 
 DateTime CreatedAt,
 string UserName
);
   

