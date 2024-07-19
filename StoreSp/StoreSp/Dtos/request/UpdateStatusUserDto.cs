﻿using System.ComponentModel.DataAnnotations;

namespace StoreSp.Dtos.request;

public record UpdateStatusUserDto
(
    [Required]
    string Email,
    [Required]
    string Phone,
    [Required]
    int Status
);
