using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using MockQueryable.Moq;
using Moq;
using TodoWebProjekt.Authorization;
using TodoWebProjekt.Models;
using TodoWebProjekt.ViewModel;
using File = TodoWebProjekt.Models.File;

namespace TodoWebProjekt.Test.Helper
{
    public class TodoControllerHelper
    {
        public static SelectList GetTestUserSelectList()
        {
            var user1 = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "Random Name"),
                new Claim(ClaimTypes.NameIdentifier, "90e309f8-da89-4d21-82f7-297bd0a2f378"),
                new Claim("custom-claim", "example claim value")
            }, "mock"));

            var user2 = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "Random Name2"),
                new Claim(ClaimTypes.NameIdentifier, "ddc052bc-70e5-462e-a071-b932ba54909e"),
                new Claim("custom-claim", "example claim value")
            }, "mock"));

            var userList = new List<ClaimsPrincipal> { user1, user2 };

            var mockSet = userList.AsQueryable().BuildMockDbSet();

            var users = mockSet.Object.OrderBy(c => c.Identity.Name)
                .Select(x => new { Id = x.FindFirstValue(ClaimTypes.NameIdentifier), Value = x.Identity.Name })
                .Where(x => x.Id != "ddc052bc-70e5-462e-a071-b932ba54909e");
            return new SelectList(users, "Id", "Value");
        }

        public static FileTaskViewModel GetFileTaskViewModel()
        {
            IFormFile file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("dummy image")), 0, 1000, "Data", "image.png")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image"
            };
            return new FileTaskViewModel
            {
                Task = new Task
                {
                    Title = "Test Eintrag",
                    Description = "Ein Eintrag für einen kleinen Test",
                    UserId = "90e309f8-da89-4d21-82f7-297bd0a2f378",
                    TaskId = 0
                },
                File = new File
                {
                    ContentType = "image/gif",
                    Filename = "NichtRandom",
                    TaskId = 1,
                    Image = ReadFile("C:\\Users\\lukas.kleybolte\\Pictures\\Forest.gif")
                },
                UploadImage = file
            };
        }

        public static FileTaskViewModel GetWrongFileWithFileTaskViewModel()
        {
            IFormFile file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("dummy pdf")), 0, 1000, "Data", "image.pdf")
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/pdf"
            };
            return new FileTaskViewModel
            {
                Task = new Task
                {
                    Title = "Test Eintrag",
                    Description = "Ein Eintrag für einen kleinen Test",
                    UserId = "90e309f8-da89-4d21-82f7-297bd0a2f378",
                    TaskId = 0
                },
                UploadImage = file,
            };
        }

        public static FileTaskViewModel GetEmptyFileWithFileTaskViewModel()
        {
            return new FileTaskViewModel
            {
                Task = new Task
                {
                    Title = "Test Eintrag",
                    Description = "Ein Eintrag für einen kleinen Test",
                    UserId = "90e309f8-da89-4d21-82f7-297bd0a2f378",
                    TaskId = 0
                },
                File = new File
                {
                    ContentType = "image/gif",
                    Filename = "NichtRandom",
                    TaskId = 1,
                    Image = ReadFile("C:\\Users\\lukas.kleybolte\\Pictures\\Forest.gif")
                },
                EmptyImage = true,
            };
        }

        private static byte[] ReadFile(string sPath)
        {
            var fInfo = new FileInfo(sPath);
            var numBytes = fInfo.Length;
            var fStream = new FileStream(sPath, FileMode.Open, FileAccess.Read);
            using var br = new BinaryReader(fStream);
            var data = br.ReadBytes((int)numBytes);
            return data;
        }

        public static ClaimsPrincipal GetTestUser()
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "Random Name"),
                new Claim(ClaimTypes.NameIdentifier, "90e309f8-da89-4d21-82f7-297bd0a2f378"),
                new Claim(ClaimTypes.Role, Constants.UserRole),
                new Claim("custom-claim", "example claim value")
            }, "mock"));
        }

        public static ClaimsPrincipal GetTestAdmin()
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "Random Name"),
                new Claim(ClaimTypes.NameIdentifier, "ddc052bc-70e5-462e-a071-b932ba54909e"),
                new Claim(ClaimTypes.Role, Constants.AdminRole),
                new Claim("custom-claim", "example claim value")
            }, "mock"));
        }


        public static ClaimsPrincipal GetTestFakeUser()
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "Random Name"),
                new Claim(ClaimTypes.NameIdentifier, "ddc052bc-70e5-462e-a071-b932ba54909e"),
                new Claim(ClaimTypes.Role, Constants.UserRole),
                new Claim("custom-claim", "example claim value")
            }, "mock"));
        }

        public static IFormFile GetMockImage()
        {
            var fileMock = new Mock<IFormFile>();
            var physicalFile = new FileInfo("C:\\Users\\lukas.kleybolte\\Pictures\\Forest.gif");
            var ms = new MemoryStream();
            using var writer = new StreamWriter(ms);
            writer.Write(physicalFile.OpenRead());
            writer.Flush();
            ms.Position = 0;
            var fileName = physicalFile.Name;
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);

            return fileMock.Object;
        }
    }
}