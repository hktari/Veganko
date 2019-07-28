using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using VegankoService.Controllers;
using VegankoService.Data.Comments;

namespace VegankoService.Tests.UnitTests
{
    [TestClass]
    public class CommentsControllerTests
    {
        private CommentsController commentsController;

        [TestInitialize]
        public void Init()
        {
            commentsController = new CommentsController(new MockCommentRepository());
        }

        [TestMethod]
        public void Get_NonExistentId_ReturnsNotFound()
        {
            Assert.IsInstanceOfType(commentsController.Get("non-existent"), typeof(NotFoundResult));
        }

        [TestMethod]
        public void Get_Existing_ReturnsMatchingId()
        {
            var comment = commentsController.Get("existing").Value;
            Assert.AreEqual("existing", comment.Id);
        }


    }
}
