﻿namespace Application.Products.Dtos.Requests;

public class CreateProductRequestDto
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public decimal Price { get; set; }
}
