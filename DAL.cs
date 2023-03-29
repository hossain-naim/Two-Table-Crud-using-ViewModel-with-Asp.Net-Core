using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace CoreNew2
{
    public class Trainer
    {
        [Key]
        public string TrainerId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public bool Active { get; set; }
        public string Image { get; set; }
        public virtual ICollection<Player> playerlink { get; set; }
    }
    public class Player
    {
        [Key]
        public string PlayerId { get; set; }
        public string slno { get; set; }
        [ForeignKey("playerlink")]
        public string TrainerId { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }

        public virtual Trainer playerlink { get; set; }
    }
    public class TrainerPlayer_VM
    {
        [Required(ErrorMessage = "Please enter Trainer Id")]
        [Display(Name = ("Trainer Id"))]
        public string TrainerId { get; set; }
        [Required(ErrorMessage = "Please enter Name")]
        public string Name { get; set; }
        public string Address { get; set; }
        public bool Active { get; set; }
        public string Image { get; set; }
        [Required(ErrorMessage = "Please enter PlayerId")]
        [Display(Name=("Player Id"))]
        public string PlayerId { get; set; }
        [Required(ErrorMessage = "Please enter slno")]
        public string slno { get; set; }
        public string Description { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
    }

}
