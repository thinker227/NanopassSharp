using System;
using System.Collections.Generic;

namespace NanopassSharp.Models;

public sealed class PassModel
{

    public string Name { get; set; } = "";
    public ModificationPassModel? Mod { get; set; }
    public string? Type { get; set; }

}

public sealed class ModificationPassModel
{

    public string? TypeName { get; set; }
    public List<ModificationModel> Add { get; set; } = new();
    public List<ModificationModel> Remove { get; set; } = new();

}

public sealed class ModificationModel
{

    public string Target { get; set; } = "";
    public string? Parameter { get; set; }
    public string? Property { get; set; }
    public string? Type { get; set; }

}
