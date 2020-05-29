using Microsoft.AspNetCore.Mvc;
using TrainingDivisionKedis.Controllers;
using Xunit;

namespace TrainingDivisionKedis.Tests.ControllersIntegrationTests
{
    public class HomeControllerTests 
    {
        HomeController _sut;

        [Fact]
        public void Index_ShouldReturnView()
        {
            _sut = new HomeController();

            var result = _sut.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
        }
    }

}
