using Xunit;
using Recreation.Models;
using System.ComponentModel.DataAnnotations;

namespace Recreation.Tests
{
    public class BusinessLogicTests
    {
        [Theory]
        [InlineData("Valid Park", "A nice park for recreation", "POINT(0 0)", true)]
        [InlineData("", "A nice park for recreation", "POINT(0 0)", false)] // Missing name
        [InlineData("Valid Park", "", "POINT(0 0)", true)] // Description is optional
        [InlineData("Valid Park", "A nice park for recreation", "", false)] // Missing geometry
        [InlineData(null, "A nice park for recreation", "POINT(0 0)", false)] // Null name
        public void AddParkModel_Validation_ReturnsExpectedResult(
            string name, 
            string description, 
            string geometry, 
            bool expectedResult)
        {
            var model = new AddParkModel
            {
                Name = name,
                Description = description,
                Geometry = geometry
            };

            var validationContext = new ValidationContext(model);
            var validationResults = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            Assert.Equal(expectedResult, isValid);
        }

        [Theory]
        [InlineData("Valid Recreation Area", "A nice area for recreation", "POLYGON((0 0, 1 0, 1 1, 0 1, 0 0))", true)]
        [InlineData("", "A nice area for recreation", "POLYGON((0 0, 1 0, 1 1, 0 1, 0 0))", false)] // Missing name
        [InlineData("Valid Recreation Area", "", "POLYGON((0 0, 1 0, 1 1, 0 1, 0 0))", true)] // Description is optional
        [InlineData("Valid Recreation Area", "A nice area for recreation", "", false)] // Missing geometry
        [InlineData(null, "A nice area for recreation", "POLYGON((0 0, 1 0, 1 1, 0 1, 0 0))", false)] // Null name
        public void AddRecreationAreaModel_Validation_ReturnsExpectedResult(
            string name, 
            string description, 
            string geometry, 
            bool expectedResult)
        {
            var model = new AddRecreationAreaModel
            {
                Name = name,
                Description = description,
                Geometry = geometry
            };

            var validationContext = new ValidationContext(model);
            var validationResults = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            Assert.Equal(expectedResult, isValid);
        }

        [Theory]
        [InlineData("Valid Bike Lane", "A safe bike lane", "LINESTRING(0 0, 1 1)", true)]
        [InlineData("", "A safe bike lane", "LINESTRING(0 0, 1 1)", false)] // Missing name
        [InlineData("Valid Bike Lane", "", "LINESTRING(0 0, 1 1)", true)] // Description is optional
        [InlineData("Valid Bike Lane", "A safe bike lane", "", false)] // Missing geometry
        [InlineData(null, "A safe bike lane", "LINESTRING(0 0, 1 1)", false)] // Null name
        public void AddBikeLaneModel_Validation_ReturnsExpectedResult(
            string name, 
            string description, 
            string geometry, 
            bool expectedResult)
        {
            var model = new AddBikeLaneModel
            {
                Name = name,
                Description = description,
                Geometry = geometry
            };

            var validationContext = new ValidationContext(model);
            var validationResults = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            Assert.Equal(expectedResult, isValid);
        }
    }
}