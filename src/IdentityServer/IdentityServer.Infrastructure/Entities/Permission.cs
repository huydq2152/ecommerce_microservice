﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Contracts.Domains;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Infrastructure.Entities;

public class Permission: EntityBase<int>
{
    public Permission(string function, string command, string roleId)
    {
        Function = function.ToUpper();
        Command = command.ToUpper();
        RoleId = roleId;
    }
    
    public Permission(int id, string function, string command, string roleId) 
        : this(function, command, roleId)
    {
        Id = id;
    }
    
    [Key]
    [MaxLength(50)]
    [Column(TypeName = "varchar(50)")]
    public string Function { get; set; }
    
    [Key]
    [MaxLength(50)]
    [Column(TypeName = "varchar(50)")]
    public string Command { get; set; }

    [ForeignKey("RoleId")]
    public string RoleId { get; set; }
    
    public virtual IdentityRole Role { get; set; }
}