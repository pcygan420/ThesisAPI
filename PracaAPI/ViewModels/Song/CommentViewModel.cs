using PracaAPI.Models.Entity;
using PracaAPI.ViewModels.Entity;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace PracaAPI.ViewModels
{
    public class CommentViewModel
    {
        public int CommentId { get; set; }
        public int EntityId { get; set; }

        [Required]
        public string UserName { get; set; }
        [Required]
        public string Text { get; set; }
        public string Date { get; set; }
        public IEnumerable<ReplyViewModel> Replies { get; set; }

        public static Comment MapToObject(CommentViewModel vm,string userId) {

            return new Comment
            {
                EntityId = vm.EntityId,
                UserId = userId,
                Text = vm.Text,
                AddDate = DateTime.Now,
                Replies = new List<Reply>()
            };
        }
    }
}