﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VegankoService.Models;

namespace VegankoService.Data.Comments
{
    public class MockCommentRepository : ICommentRepository
    {
        public void Create(Comment comment)
        {
            
        }

        public void Delete(string id)
        {
            
        }

        public Comment Get(string id)
        {
            if (id == "existing")
            {
                return new Comment { Id = "existing" };
            }

            return null;
        }

        public PagedList<Comment> GetAll(int page, int pageSize = 10)
        {
            throw new NotImplementedException();
        }

        public void Update(Comment comment)
        {
            throw new NotImplementedException();
        }
    }
}
