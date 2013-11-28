using Bookland.Areas.Admin.Controllers;
using Bookland.Areas.Admin.Models;
using Bookland.DAL.Abstract;
using Bookland.Data_Structures;
using Bookland.Helpers;
using Bookland.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Bookland.Tests.AdminTests
{
    [TestClass]
    public class ProductTests
    {
        private Mock<IProductRepository> mockProductRepository;
        private Mock<ICategoryRepository> mockCategoryRepository;
        private ProductController controller;
        private static Category exampleCategory = new Category { CategoryID = 1, CategoryName = "ROOT" };
        private static Product exampleProduct = new Product { ProductID = 1, Name = "exampleProduct", Category = exampleCategory };
        private static TreeNode<Category> exampleCategoryTree = new TreeNode<Category>(exampleCategory);

        [TestInitialize]
        public void TestInitialize()
        {
            mockProductRepository = new Mock<IProductRepository>();
            mockCategoryRepository = new Mock<ICategoryRepository>();
            controller = new ProductController(mockProductRepository.Object, mockCategoryRepository.Object);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            mockProductRepository = null;
            mockCategoryRepository = null;
            
            controller.Dispose();
            controller = null;
        }

        #region INDEX action tests

        #endregion

        #region CREATE action tests

        [TestMethod]
        public void GET_Create_Returns_Editor_View()
        {
            // ARRANGE
            mockCategoryRepository.Setup(m => m.GetCategoryTree(It.Is<int>(id => id == exampleCategoryTree.Data.CategoryID)))
                .Returns(exampleCategoryTree);

            // ACT
            var result = controller.Create();

            // ASSERT
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreEqual("Editor", ((ViewResult)result).ViewName);
        }

        [TestMethod]
        public void POST_Create_Commits_Changes_When_Successful()
        {
            // ARRANGE
            // Already handled by 'TestInitialize' method

            // ACT
            controller.Create(exampleProduct, exampleCategory.CategoryID, null);

            // ASSERT
            // Ensure that the repository's 'CreateProduct' and 'Commit' methods were successfully called
            mockProductRepository.Verify(m => m.CreateProduct(exampleProduct), Times.Once);
            mockProductRepository.Verify(m => m.Commit(), Times.Once);
        }

        [TestMethod]
        public void POST_Create_Returns_Redirect_To_Product_Index_Action_When_Successful()
        {
            // ARRANGE
            // Already handled by 'TestInitialize' method

            // ACT
            var result = controller.Create(exampleProduct, exampleCategory.CategoryID, null);

            // ASSERT
            // Ensure that the object returned is a redirect result, with the correct route values
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.AreEqual("Index", ((RedirectToRouteResult)result).RouteValues["action"]);
            Assert.AreEqual("Product", ((RedirectToRouteResult)result).RouteValues["controller"]);
        }

        [TestMethod]
        public void POST_Create_Fails_To_Call_CreateProduct_Or_Commit_Upon_Model_Error()
        {
            // ARRANGE
            // Add an error to the model
            mockCategoryRepository.Setup(m => m.GetCategoryTree(It.Is<int>(id => id == exampleCategoryTree.Data.CategoryID)))
                .Returns(exampleCategoryTree);
            controller.ModelState.AddModelError("SomeError", "Error occurred");

            // ACT
            controller.Create(exampleProduct, exampleCategory.CategoryID, null);

            // ASSERT
            // Ensure that 'CreateProduct' and 'Commit' methods were not called successfully
            mockProductRepository.Verify(m => m.CreateProduct(exampleProduct), Times.Never);
            mockProductRepository.Verify(m => m.Commit(), Times.Never);
        }

        [TestMethod]
        public void POST_Create_Returns_Editor_View_Upon_Model_Error()
        {
            // ARRANGE
            mockCategoryRepository.Setup(m => m.GetCategoryTree(It.Is<int>(id => id == exampleCategoryTree.Data.CategoryID)))
                .Returns(exampleCategoryTree);
            controller.ModelState.AddModelError("SomeError", "Error occurred");

            // ACT
            var result = controller.Create(exampleProduct, exampleCategory.CategoryID, null);

            // ASSERT
            // Ensure that the object returned is a view result
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            // Ensure that it is the 'Editor' view being returned
            Assert.AreEqual("Editor", ((ViewResult)result).ViewName);
        }

        [TestMethod]
        public void POST_Create_Returns_ProductEditorViewModel_Upon_Model_Error()
        {
            // ARRANGE
            controller.ModelState.AddModelError("SomeError", "Error occurred.");
            mockCategoryRepository.Setup(m => m.GetCategoryTree(It.Is<int>(id => id == exampleCategoryTree.Data.CategoryID)))
                .Returns(exampleCategoryTree);

            // ACT
            var result = controller.Create(exampleProduct, exampleCategory.CategoryID, null);

            // ASSERT
            // Ensure that the Product object passed in to the controller action and the view's model are identical
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var model = ((ViewResult)result).ViewData.Model;
            Assert.IsInstanceOfType(model, typeof(ProductEditorViewModel));
            Assert.AreSame(exampleProduct, ((ProductEditorViewModel)model).Product);
            Assert.AreEqual("Create", ((ProductEditorViewModel)model).Action);
        }

        [TestMethod]
        public void POST_Create_Catches_And_Handles_DataException()
        {
            // ARRANGE
            // Set-up the 'CreateProduct' method with throwing of DataException
            mockProductRepository.Setup(m => m.CreateProduct(It.IsAny<Product>()))
                .Throws<DataException>();
            mockCategoryRepository.Setup(m => m.GetCategoryTree(It.Is<int>(id => id == exampleCategoryTree.Data.CategoryID)))
                .Returns(exampleCategoryTree);

            // ACT
            controller.Create(exampleProduct, exampleCategory.CategoryID, null);

            // ASSERT
            // Ensure that 'CreateProduct' was called successfully (i.e. caught and handled the exception)
            mockProductRepository.Verify(m => m.CreateProduct(exampleProduct), Times.Once);

            // Ensure that the subsequent 'Commit' method call wasn't called (i.e. immediately directs to catch block)
            mockProductRepository.Verify(m => m.Commit(), Times.Never);
        }

        [TestMethod]
        public void POST_Create_Returns_Editor_View_Upon_DataException()
        {
            // ARRANGE
            mockProductRepository.Setup(m => m.CreateProduct(It.IsAny<Product>()))
                .Throws<DataException>();
            mockCategoryRepository.Setup(m => m.GetCategoryTree(It.Is<int>(id => id == exampleCategoryTree.Data.CategoryID)))
                .Returns(exampleCategoryTree);

            // ACT
            var result = controller.Create(exampleProduct, exampleCategory.CategoryID, null);

            // ASSERT
            // Ensure that the object returned is a view result
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            // Ensure that it is the 'Editor' view being returned
            Assert.AreEqual("Editor", ((ViewResult)result).ViewName);
        }

        [TestMethod]
        public void POST_Create_Returns_ProductEditorViewModel_Upon_DataException()
        {
            // ARRANGE
            mockProductRepository.Setup(m => m.CreateProduct(It.IsAny<Product>()))
                .Throws<DataException>();
            mockCategoryRepository.Setup(m => m.GetCategoryTree(It.Is<int>(id => id == exampleCategoryTree.Data.CategoryID)))
                .Returns(exampleCategoryTree);

            // ACT
            var result = controller.Create(exampleProduct, exampleCategory.CategoryID, null);

            // ASSERT
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var model = ((ViewResult)result).ViewData.Model;
            Assert.IsInstanceOfType(model, typeof(ProductEditorViewModel));
            Assert.AreSame(exampleProduct, ((ProductEditorViewModel)model).Product);
            Assert.AreEqual("Create", ((ProductEditorViewModel)model).Action);
        }

        #endregion

        #region UPDATE action tests

        [TestMethod]
        public void GET_Update_Calls_GetProduct()
        {
            // ARRANGE
            // Set up the mock repository's 'GetProduct' method (which is called within 'Update') with mock Product data
            mockProductRepository.Setup(m => m.GetProduct(It.Is<int>(pID => pID == exampleProduct.ProductID)))
                .Returns(exampleProduct);
            mockCategoryRepository.Setup(m => m.GetCategoryTree(It.Is<int>(id => id == exampleCategoryTree.Data.CategoryID)))
                .Returns(exampleCategoryTree);

            // ACT
            controller.Update(exampleProduct.ProductID);

            // ASSERT
            // Verify that the 'GetProduct' method was successfully called
            mockProductRepository.Verify(m => m.GetProduct(exampleProduct.ProductID), Times.Once);
        }

        [TestMethod]
        public void GET_Update_Returns_ProductEditorViewModel()
        {
            // ARRANGE
            mockProductRepository.Setup(m => m.GetProduct(It.Is<int>(pID => pID == exampleProduct.ProductID)))
                .Returns(exampleProduct);
            mockCategoryRepository.Setup(m => m.GetCategoryTree(It.Is<int>(id => id == exampleCategoryTree.Data.CategoryID)))
                .Returns(exampleCategoryTree);

            // ACT
            var result = controller.Update(exampleProduct.ProductID).ViewData.Model;

            // ASSERT
            // Check that the view's model is identical to the one returned by the 'GetProduct' method
            Assert.IsInstanceOfType(result, typeof(ProductEditorViewModel));
            Assert.AreSame(exampleProduct, ((ProductEditorViewModel)result).Product);
            Assert.AreEqual("Update", ((ProductEditorViewModel)result).Action);
        }

        [TestMethod]
        public void GET_Update_Returns_Editor_View()
        {
            // ARRANGE
            mockProductRepository.Setup(m => m.GetProduct(It.Is<int>(pID => pID == exampleProduct.ProductID)))
                .Returns(exampleProduct);
            mockCategoryRepository.Setup(m => m.GetCategoryTree(It.Is<int>(id => id == exampleCategoryTree.Data.CategoryID)))
                .Returns(exampleCategoryTree);

            // ACT
            var result = controller.Update(exampleProduct.ProductID).ViewName;

            // ASSERT
            // Ensure that it is the 'Editor' view being returned
            Assert.AreEqual("Editor", result);
        }

        [TestMethod]
        public void POST_Update_Commits_Changes_When_Successful()
        {
            // ARRANGE
            // Already handled by 'TestInitialize' method

            // ACT
            controller.Update(exampleProduct, exampleCategory.CategoryID, null);

            // ASSERT
            // Ensure that the repository's 'UpdateProduct' and 'Commit' methods were successfully called
            mockProductRepository.Verify(m => m.UpdateProduct(exampleProduct), Times.Once);
            mockProductRepository.Verify(m => m.Commit(), Times.Once);
        }

        [TestMethod]
        public void POST_Update_Returns_Redirect_To_Product_Index_Action_When_Successful()
        {
            // ARRANGE
            // Already handled by 'TestInitialize' method

            // ACT
            var result = controller.Update(exampleProduct, exampleCategory.CategoryID, null);

            // ASSERT
            // Ensure that the object returned is a redirect result, with the correct route values
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.AreEqual("Index", ((RedirectToRouteResult)result).RouteValues["action"]);
            Assert.AreEqual("Product", ((RedirectToRouteResult)result).RouteValues["controller"]);
        }

        [TestMethod]
        public void POST_Update_Fails_To_Call_UpdateProduct_Or_Commit_Upon_Model_Error()
        {
            // ARRANGE
            // Add an error to the model state
            mockCategoryRepository.Setup(m => m.GetCategoryTree(It.Is<int>(id => id == exampleCategoryTree.Data.CategoryID)))
                .Returns(exampleCategoryTree);
            controller.ModelState.AddModelError("SomeError", "Error occured.");
            
            // ACT
            // Attempt to update with the product
            controller.Update(exampleProduct, exampleCategory.CategoryID, null);

            // ASSERT
            // Ensure that 'UpdateProduct' and 'Commit' methods were not called successfully
            mockProductRepository.Verify(m => m.UpdateProduct(exampleProduct), Times.Never);
            mockProductRepository.Verify(m => m.Commit(), Times.Never);
        }

        [TestMethod]
        public void POST_Update_Returns_Editor_View_Upon_Model_Error()
        {
            // ARRANGE
            controller.ModelState.AddModelError("SomeError", "Error occured.");
            mockCategoryRepository.Setup(m => m.GetCategoryTree(It.Is<int>(id => id == exampleCategoryTree.Data.CategoryID)))
                .Returns(exampleCategoryTree);
            
            // ACT
            var result = controller.Update(exampleProduct, exampleCategory.CategoryID, null);

            // ASSERT
            // Ensure that the object returned is a view result
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            // Ensure that it is the 'Editor' view being returned
            Assert.AreEqual("Editor", ((ViewResult)result).ViewName);
        }

        [TestMethod]
        public void POST_Update_Returns_ProductEditorViewModel_Upon_Model_Error()
        {
            // ARRANGE
            mockCategoryRepository.Setup(m => m.GetCategoryTree(It.Is<int>(id => id == exampleCategoryTree.Data.CategoryID)))
                .Returns(exampleCategoryTree);
            controller.ModelState.AddModelError("SomeError", "Error occured.");

            // ACT
            var result = controller.Update(exampleProduct, exampleCategory.CategoryID, null);

            // ASSERT
            // Ensure that the view's model is identical to the one passed in to the 'Update' method
            Assert.IsInstanceOfType(((ViewResult)result).ViewData.Model, typeof(ProductEditorViewModel));

            ProductEditorViewModel model = ((ViewResult)result).ViewData.Model as ProductEditorViewModel;
            Assert.AreSame(exampleProduct, model.Product);
            Assert.AreEqual("Update", model.Action);
        }

        [TestMethod]
        public void POST_Update_Catches_And_Handles_DataException()
        {
            // ARRANGE
            // Set-up the mock repository's 'UpdateProduct' method with throwing of DataException
            mockProductRepository.Setup(m => m.UpdateProduct(It.IsAny<Product>()))
                .Throws<DataException>();
            mockCategoryRepository.Setup(m => m.GetCategoryTree(It.Is<int>(id => id == exampleCategoryTree.Data.CategoryID)))
                .Returns(exampleCategoryTree);

            // ACT
            controller.Update(exampleProduct, exampleCategory.CategoryID, null);

            // ASSERT
            // Ensure that 'UpdateProduct' was called successfully (i.e. caught and handled the exception)
            mockProductRepository.Verify(m => m.UpdateProduct(exampleProduct), Times.Once);

            // Ensure that the subsequent 'Commit' method call wasn't called (i.e. immediately directs to catch block after 'UpdateProduct' throws DataException)
            mockProductRepository.Verify(m => m.Commit(), Times.Never);
        }

        [TestMethod]
        public void POST_Update_Contains_Model_Error_Upon_DataException()
        {
            // ARRANGE
            mockProductRepository.Setup(m => m.UpdateProduct(It.IsAny<Product>()))
                .Throws<DataException>();
            mockCategoryRepository.Setup(m => m.GetCategoryTree(It.Is<int>(id => id == exampleCategoryTree.Data.CategoryID)))
                .Returns(exampleCategoryTree);

            // ACT
            controller.Update(exampleProduct, exampleCategory.CategoryID, null);

            // ASSERT
            // Ensure that the model error was successfully added and that the the model state is invalid
            Assert.IsNotNull(controller.ModelState["DbError"]);
            Assert.IsFalse(controller.ModelState.IsValid);
        }

        [TestMethod]
        public void POST_Update_Returns_Editor_View_Upon_DataException()
        {
            // ARRANGE
            mockProductRepository.Setup(m => m.UpdateProduct(It.IsAny<Product>()))
                .Throws<DataException>();
            mockCategoryRepository.Setup(m => m.GetCategoryTree(It.Is<int>(id => id == exampleCategoryTree.Data.CategoryID)))
                .Returns(exampleCategoryTree);

            // ACT
            var result = controller.Update(exampleProduct, exampleCategory.CategoryID, null);

            // ASSERT
            // Ensure that the object returned is a view result
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            // Ensure that it is the 'Editor' view being returned
            Assert.AreEqual("Editor", ((ViewResult)result).ViewName);
        }

        [TestMethod]
        public void POST_Update_Returns_ProductEditorViewModel_Upon_DataException()
        {
            // ARRANGE
            mockProductRepository.Setup(m => m.UpdateProduct(It.IsAny<Product>()))
                .Throws<DataException>();
            mockCategoryRepository.Setup(m => m.GetCategoryTree(It.Is<int>(id => id == exampleCategoryTree.Data.CategoryID)))
                .Returns(exampleCategoryTree);

            // ACT
            var result = controller.Update(exampleProduct, exampleCategory.CategoryID, null);

            // ASSERT
            // Ensure that the view's model is identical to the one passed in to the 'Update' method
            Assert.IsInstanceOfType(((ViewResult)result).ViewData.Model, typeof(ProductEditorViewModel));

            ProductEditorViewModel model = ((ViewResult)result).ViewData.Model as ProductEditorViewModel;
            Assert.AreSame(exampleProduct, model.Product);
            Assert.AreSame("Update", model.Action);
        }

        #endregion

        #region DELETE action tests

        [TestMethod]
        public void GET_Delete_Calls_GetProduct()
        {
            // ARRANGE
            // Already handled by 'TestInitialize' method

            // ACT
            var result = controller.Delete(exampleProduct.ProductID);

            // ASSERT
            // Ensure that the 'GetProduct' method was called successfully
            mockProductRepository.Verify(m => m.GetProduct(exampleProduct.ProductID), Times.Once);
        }

        [TestMethod]
        public void GET_Delete_Returns_Product_Model()
        {
            // ARRANGE
            mockProductRepository.Setup(m => m.GetProduct(It.Is<int>(pID => pID == exampleProduct.ProductID)))
                .Returns(exampleProduct);

            // ACT
            var result = controller.Delete(exampleProduct.ProductID).ViewData.Model;

            // ASSERT
            // Ensure that the passed Product object and the view's model are identical
            Assert.IsInstanceOfType(result, typeof(Product));
            Assert.AreSame(exampleProduct, result);
        }

        [TestMethod]
        public void POST_Delete_Commits_Deletion_When_Successful()
        {
            // ARRANGE
            // Already handled by 'TestInitialize' method

            // ACT
            // Call the POST 'Delete' method
            controller.Delete(exampleProduct.ProductID, exampleProduct.Name);

            // ASSERT
            mockProductRepository.Verify(m => m.DeleteProduct(exampleProduct.ProductID), Times.Once);
            mockProductRepository.Verify(m => m.Commit(), Times.Once);
        }

        [TestMethod]
        public void POST_Delete_Returns_Redirect_To_Product_Index_Action_When_Successful()
        {
            // ARRANGE
            // Already handled by 'TestInitialize' method

            // ACT
            var result = controller.Delete(exampleProduct.ProductID, exampleProduct.Name).RouteValues;

            // ASSERT
            // Ensure that the redirect has the correct route values
            Assert.AreEqual("Index", result["action"]);
            Assert.AreEqual("Product", result["controller"]);

            // Ensure that a TempData message was created (for redirect's result), with the intended contents
            Assert.AreEqual("'" + exampleProduct.Name + "' product successfully deleted from the database.", 
                controller.TempData["message"]);
        }

        [TestMethod]
        public void POST_Delete_Catches_And_Handles_DataException()
        {
            // ARRANGE
            // Set-up the mock repository's 'DeleteProduct' method with throwing of DataException
            mockProductRepository.Setup(m => m.DeleteProduct(It.IsAny<int>())).Throws<DataException>();

            // ACT
            controller.Delete(exampleProduct.ProductID, exampleProduct.Name);

            // ASSERT
            // Ensure that 'DeleteProduct' was called successfully (i.e. caught and handled the exception)
            mockProductRepository.Verify(m => m.DeleteProduct(exampleProduct.ProductID), Times.Once);

            // Ensure that the subsequent 'Commit' method call wasn't called (i.e. immediately directs to catch block)
            mockProductRepository.Verify(m => m.Commit(), Times.Never);

            // Ensure that a TempData message was created (for redirect's result), with the intended contents
            Assert.AreEqual("Deletion was not successful. Please contact your system admin if problems persist.",
                controller.TempData["message"]);
        }

        [TestMethod]
        public void POST_Delete_Returns_Redirect_To_Delete_Action_For_Product_Upon_DataException()
        {
            // ARRANGE
            // Set-up the mock repository's 'DeleteProduct' method with throwing of DataException
            mockProductRepository.Setup(m => m.DeleteProduct(It.IsAny<int>())).Throws<DataException>();

            // ACT
            var result = controller.Delete(exampleProduct.ProductID, exampleProduct.Name).RouteValues;

            // ASSERT
            // Ensure that the redirect has the correct route values
            Assert.AreEqual("Delete", result["action"]);
            Assert.AreEqual(exampleProduct.ProductID, result["productID"]);
        }

        #endregion
    }
}