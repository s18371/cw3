using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

public class PostProm
{
		
		[Required]
		public string Studies { get; set; }
		[Required]
		public int Semester { get; set; }
}
	


