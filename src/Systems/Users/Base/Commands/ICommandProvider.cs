using Amethyst.Systems.Commands.Base;
using Amethyst.Text;

namespace Amethyst.Systems.Users.Base.Commands;

public interface ICommandProvider
{
    public CommandHistory History { get; }

    public PagesCollection? ActivePage { get; set; }

    public int Delay { get; set; }

    public List<string> Repositories { get; }

    public void RunCommand(string commandText);
    public void RunCommand(ICommand command, string commandArgs);
}
