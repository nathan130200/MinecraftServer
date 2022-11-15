using System.Runtime.Serialization;

namespace MinecraftServer.Entities.Chat;

public enum ClickComponentAction
{
    [EnumMember(Value = "open_url")]
    OpenUrl,

    [EnumMember(Value = "run_command")]
    RunCommand,

    [EnumMember(Value = "suggest_command")]
    SuggestCommand,

    [EnumMember(Value = "change_page")]
    ChangeBookPage,

    [EnumMember(Value = "copy_to_clipboard")]
    CopyToClipboard
}
