using System.Runtime.Serialization;

namespace MinecraftServer.Entities.Chat;

public enum HoverComponentAction
{
    [EnumMember(Value = "show_text")]
    ShowText,

    [EnumMember(Value = "show_item")]
    ShowItem,

    [EnumMember(Value = "show_entity")]
    ShowEntity
}