using System.ComponentModel.DataAnnotations;

namespace MinimalAPIDemo.Models
{
    public class Employee
    {
        // Employee identifier, auto-generated internally, no validation needed here
        public int Id { get; set; }
        // Name is required and cannot exceed 100 characters
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = null!;
        // Position is required and cannot exceed 50 characters
        [Required(ErrorMessage = "Position is required")]
        [StringLength(50, ErrorMessage = "Position cannot exceed 50 characters")]
        public string Position { get; set; } = null!;
        // Salary must be within a realistic range
        [Required(ErrorMessage = "Salary is required")]
        [Range(30000, 200000, ErrorMessage = "Salary must be between 30,000 and 200,000")]
        public decimal Salary { get; set; }
    }
}
