using Xunit;
using BookStoreApp.WebApi.Controllers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BookStoreApp.WebApi.Models;
using BookStoreApp.Tests.Helper;

namespace BookStoreApp.Tests
{
    public class ControllerTests
    {
        [Fact]
        public async Task BookStoreController_Get_Should_Return_ActionResult()
        {
            //Arrange
            var controller = new BookStoreController();

            //Act
            var result = await controller.Get();

            //Assert
            Assert.IsType<JsonResult>(result);
        }

        [Fact]
        public async Task BookStoreController_Get_Should_Return_Correct_BookStore_Data()
        {
            //Arrange
            var controller = new BookStoreController();

            //Act


            var result = await controller.Get();
            var json = result.ToJson<BookStore>();

            //Assert
            Assert.True(json[0].Name == "Waterstones",$"Assert failed, received {json[0].Name} ");
            Assert.True(json[0].PostCode == "PO19 1QD",$"Assert failed, received {json[0].PostCode} ");
            Assert.True(json[0].TelephoneNumber == "01234 773030",$"Assert failed, received {json[0].TelephoneNumber} ");
        }
    }
}
