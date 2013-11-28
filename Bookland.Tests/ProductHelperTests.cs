using Bookland.DAL.Abstract;
using Bookland.Helpers;
using Bookland.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.IO;
using System.Web;

namespace Bookland.Tests
{
    [TestClass]
    public class ProductHelperTests
    {
        #region ProductsByOrder tests

        [TestMethod]
        public void ProductsByOrder_With_Name_Ascending_Order_Parameter_Calls_GetProducts_Correctly()
        {
            // ARRANGE
            Mock<IProductRepository> mockProductRepository = new Mock<IProductRepository>();

            // ACT
            ProductHelpers.ProductsByOrder(mockProductRepository.Object, ProductHelpers.NameAsc);

            // ASSERT
            mockProductRepository.Verify(m => m.GetProducts(p => p.Name, false, null));
        }

        [TestMethod]
        public void ProductsByOrder_With_Name_Descending_Order_Parameter_Calls_GetProducts_Correctly()
        {
            // ARRANGE
            Mock<IProductRepository> mockProductRepository = new Mock<IProductRepository>();

            // ACT
            ProductHelpers.ProductsByOrder(mockProductRepository.Object, ProductHelpers.NameDesc);

            // ASSERT
            mockProductRepository.Verify(m => m.GetProducts(p => p.Name, true, null));
        }

        [TestMethod]
        public void ProductsByOrder_With_ProductID_Ascending_Order_Parameter_Calls_GetProducts_Correctly()
        {
            // ARRANGE
            Mock<IProductRepository> mockProductRepository = new Mock<IProductRepository>();

            // ACT
            ProductHelpers.ProductsByOrder(mockProductRepository.Object, ProductHelpers.IdAsc);

            // ASSERT
            mockProductRepository.Verify(m => m.GetProducts(p => p.ProductID, false, null));
        }

        [TestMethod]
        public void ProductsByOrder_With_ProductID_Descending_Order_Parameter_Calls_GetProducts_Correctly()
        {
            // ARRANGE
            Mock<IProductRepository> mockProductRepository = new Mock<IProductRepository>();

            // ACT
            ProductHelpers.ProductsByOrder(mockProductRepository.Object, ProductHelpers.IdDesc);

            // ASSERT
            mockProductRepository.Verify(m => m.GetProducts(p => p.ProductID, true, null));
        }

        [TestMethod]
        public void ProductsByOrder_With_Price_Ascending_Order_Parameter_Calls_GetProducts_Correctly()
        {
            // ARRANGE
            Mock<IProductRepository> mockProductRepository = new Mock<IProductRepository>();

            // ACT
            ProductHelpers.ProductsByOrder(mockProductRepository.Object, ProductHelpers.PriceAsc);

            // ASSERT
            mockProductRepository.Verify(m => m.GetProducts(p => p.Price, false, null));
        }

        [TestMethod]
        public void ProductsByOrder_With_Price_Descending_Order_Parameter_Calls_GetProducts_Correctly()
        {
            // ARRANGE
            Mock<IProductRepository> mockProductRepository = new Mock<IProductRepository>();

            // ACT
            ProductHelpers.ProductsByOrder(mockProductRepository.Object, ProductHelpers.PriceDesc);

            // ASSERT
            mockProductRepository.Verify(m => m.GetProducts(p => p.Price, true, null));
        }

        [TestMethod]
        public void ProductsByOrder_With_DateAdded_Ascending_Order_Parameter_Calls_GetProducts_Correctly()
        {
            // ARRANGE
            Mock<IProductRepository> mockProductRepository = new Mock<IProductRepository>();

            // ACT
            ProductHelpers.ProductsByOrder(mockProductRepository.Object, ProductHelpers.DateAddedAsc);

            // ASSERT
            mockProductRepository.Verify(m => m.GetProducts(p => p.DateAdded, false, null));
        }

        [TestMethod]
        public void ProductsByOrder_With_DateAdded_Descending_Order_Parameter_Calls_GetProducts_Correctly()
        {
            // ARRANGE
            Mock<IProductRepository> mockProductRepository = new Mock<IProductRepository>();

            // ACT
            ProductHelpers.ProductsByOrder(mockProductRepository.Object, ProductHelpers.DateAddedDesc);

            // ASSERT
            mockProductRepository.Verify(m => m.GetProducts(p => p.DateAdded, true, null));
        }

        [TestMethod]
        public void ProductsByOrder_With_Null_Order_Parameter_Throws_ArgumentNullException()
        {
            // ARRANGE
            Mock<IProductRepository> mockProductRepository = new Mock<IProductRepository>();

            // ACT
            Exception result = null;
            try
            {
                ProductHelpers.ProductsByOrder(mockProductRepository.Object, null);
            }
            catch (Exception ex)
            {
                result = ex;
            }

            // ASSERT
            Assert.IsInstanceOfType(result, typeof(ArgumentNullException));
            Assert.IsTrue(result.Message.Contains("order cannot be null."));
        }

        [TestMethod]
        public void ProductsByOrder_With_Invalid_Order_Parameter_Throws_ArgumentException()
        {
            // ARRANGE
            Mock<IProductRepository> mockProductRepository = new Mock<IProductRepository>();

            // ACT
            Exception result = null;
            try
            {
                ProductHelpers.ProductsByOrder(mockProductRepository.Object, "nme_ascc");
            }
            catch (Exception ex)
            {
                result = ex;
            }

            // ASSERT
            Assert.IsInstanceOfType(result, typeof(ArgumentException));
            Assert.IsTrue(result.Message.Contains("order value is invalid."));
        }

        #endregion

        #region SetProductImage tests

        private static readonly byte[] imageData = { 1, 2, 3, 4 };
        private const string mimeType = "image/jpeg";
        private static readonly Product exampleProduct = new Product { ProductID = 1, Name = "Example Product" };

        [TestMethod]
        public void SetProductImage_Sets_Image_Information_For_Product()
        {
            // ARRANGE
            // Create a mock image (N.B. HttpPostedFileBase cannot be instantiated traditionally - properties are read-only)
            Mock<HttpPostedFileBase> mockImage = new Mock<HttpPostedFileBase>();
            mockImage.Setup(m => m.InputStream).Returns(new MemoryStream(imageData));
            mockImage.Setup(m => m.ContentType).Returns(mimeType);

            // ACT
            Product result = ProductHelpers.SetProductImage(exampleProduct, mockImage.Object);

            // ASSERT
            // First, prepare an expected product, and add the expected image into it
            Product expectedProduct = exampleProduct;
            expectedProduct.ImageData = imageData;
            expectedProduct.ImageMimeType = mimeType;

            // Ensure that the expected product and actual/result product are identical
            Assert.AreSame(expectedProduct, result);
        }

        /// <summary>
        /// A helper method for exception-expecting SetProductImage unit tests. Represents their 'Act' stage.
        /// </summary>
        /// <param name="image">Image file to be passed into 'SetProductImage' call.</param>
        /// <returns>The exception caught from calling 'SetProductImage', or lack thereof (i.e. null).</returns>
        private Exception Attempt_To_Catch_SetProductImage_Exception(HttpPostedFileBase image)
        {
            Exception result = null;
            try
            {
                ProductHelpers.SetProductImage(exampleProduct, image);
            }
            catch (Exception ex)
            {
                result = ex;
            }

            return result;
        }

        [TestMethod]
        public void SetProductImage_Throws_ArgumentNullException_Upon_Null_productImage()
        {
            // ARRANGE
            // Example Product object

            // ACT
            Exception result = Attempt_To_Catch_SetProductImage_Exception(null);

            // ASSERT
            // Ensure that the exception is an ArgumentNullException, and that it contains the intended exception message
            Assert.IsInstanceOfType(result, typeof(ArgumentNullException));
            Assert.IsTrue(result.Message.Contains("productImage cannot be null."));
        }

        [TestMethod]
        public void SetProductImage_Throws_ArgumentException_Upon_Null_InputStream()
        {
            // ARRANGE
            // Create a mock image that can't return image data
            Mock<HttpPostedFileBase> mockImage = new Mock<HttpPostedFileBase>();
            mockImage.Setup(m => m.ContentType).Returns(mimeType);

            // ACT
            Exception result = Attempt_To_Catch_SetProductImage_Exception(mockImage.Object);

            // ASSERT
            // Ensure that the exception is an ArgumentException, and that it contains the intended exception message
            Assert.IsInstanceOfType(result, typeof(ArgumentException));
            Assert.IsTrue(result.Message.Contains("productImage's InputStream property cannot be null."));
        }

        [TestMethod]
        public void SetProductImage_Throws_ArgumentException_Upon_Null_ContentType()
        {
            // ARRANGE
            // Create a mock image that can't return image MIME type
            Mock<HttpPostedFileBase> mockImage = new Mock<HttpPostedFileBase>();
            mockImage.Setup(m => m.InputStream).Returns(new MemoryStream(imageData));

            // ACT
            Exception result = Attempt_To_Catch_SetProductImage_Exception(mockImage.Object);

            // ASSERT
            Assert.IsInstanceOfType(result, typeof(ArgumentException));
            Assert.IsTrue(result.Message.Contains("productImage's ContentType property cannot be null."));
        }

        #endregion
    }
}
