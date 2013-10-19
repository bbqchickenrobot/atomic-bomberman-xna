﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;
using BomberEngine.Consoles;
using BomberEngine.Core.IO;
using BomberEngine.Core.Input;

namespace BomberEngine.Consoles
{
    public class Cmd_listcmds : CCommand
    {
        public Cmd_listcmds()
            : base("listcmds")
        {
        }

        public override void Execute()
        {
            String prefix = StrArg(0);

            List<CCommand> commands = prefix != null ? console.ListCommands(prefix) : console.ListCommands();
            commands.Sort(CompareCommands);

            for (int i = 0; i < commands.Count; ++i)
            {
                Print(commands[i].name);
            }
            Print(commands.Count + " commands");
        }

        private int CompareCommands(CCommand a, CCommand b)
        {
            return a.name.CompareTo(b.name);
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    public class Cmd_listcvars : CCommand
    {
        public Cmd_listcvars()
            : base("listcvars")
        {
        }

        public override void Execute()
        {
            String prefix = StrArg(0);

            List<CVar> vars = prefix != null ? console.ListVars(prefix) : console.ListVars();
            vars.Sort(CompareVars);

            for (int i = 0; i < vars.Count; ++i)
            {
                Print(vars[i].name);
            }
            Print(vars.Count + " vars");
        }

        private int CompareVars(CVar a, CVar b)
        {
            return a.name.CompareTo(b.name);
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    public class Cmd_exit : CCommand
    {
        public Cmd_exit()
            : base("exit")
        {
        }

        public override void Execute()
        {
            Application.sharedApplication.Stop();
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    public class Cmd_exec : CCommand
    {
        public Cmd_exec()
            : base("exec")
        {
        }

        public override void Execute()
        {
            if (ArgsCount() != 1)
            {
                Print("usage: " + name + " <filename>");
                return;
            }

            String filename = StrArg(0);
            List<String> lines = FileUtils.Read(filename);

            if (lines == null)
            {
                Print("Can't read file: " + filename);
                return;
            }

            foreach (String line in lines)
            {
                String trim = line.Trim();
                if (trim.Length == 0 || trim.StartsWith("//"))
                {
                    continue;
                }

                console.TryExecuteCommand(line);
            }

            console.Print("Load config " + filename);
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    public class Cmd_write : CCommand
    {
        public Cmd_write()
            : base("write")
        {
        }

        public override void Execute()
        {
            if (ArgsCount() != 1)
            {
                Print("usage: " + name + " <filename>");
                return;
            }

            String filename = StrArg(0);
            
            List<String> lines = new List<String>();
            lines.Add("// Generated by engine");

            List<CVar> vars = console.ListVars();
            foreach (CVar cvar in vars)
            {
                if (!cvar.IsDefault())
                {
                    lines.Add(cvar.name + " " + cvar.value);
                }
            }

            List<CKeyBinding> bindlist = GetRootController().GetKeyBindings().ListBindings();
            if (bindlist.Count > 0)
            {
                lines.Add("");
                lines.Add("// bindings");
                for (int i = 0; i < bindlist.Count; ++i)
                {
                    lines.Add("bind " + bindlist[i].name + " \"" + bindlist[i].cmd + "\"");
                }
            }

            FileUtils.Write(filename, lines);
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    public class Cmd_bind : CCommand
    {
        public Cmd_bind()
            : base("bind")
        {
        }

        public override void Execute()
        {
            if (ArgsCount() != 2)
            {
                Print("usage: " + name + " <key> <command>");
                return;
            }

            String codeValue = StrArg(0).ToUpper();
            KeyCode code = KeyCodeHelper.FromString(codeValue);
            if (code == KeyCode.None)
            {
                Print("Invalid key '" + codeValue + "'");
                return;
            }

            String cmd = StrArg(1);

            CKeyBindings bindings = GetRootController().GetKeyBindings();
            bindings.Bind(code, cmd);
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    public class Cmd_unbind : CCommand
    {
        public Cmd_unbind()
            : base("unbind")
        {
        }

        public override void Execute()
        {
            if (ArgsCount() != 1)
            {
                Print("usage: " + name + " <key>");
                return;
            }

            String codeValue = StrArg(0).ToUpper();
            KeyCode code = KeyCodeHelper.FromString(codeValue);
            if (code == KeyCode.None)
            {
                Print("Invalid key '" + codeValue + "'");
                return;
            }

            CKeyBindings bindings = GetRootController().GetKeyBindings();
            bindings.Unbind(code);
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    public class Cmd_unbind_all : CCommand
    {
        public Cmd_unbind_all()
            : base("unbind_all")
        {
        }

        public override void Execute()
        {
            if (ArgsCount() != 0)
            {
                Print("usage: " + name);
                return;
            }

            CKeyBindings bindings = GetRootController().GetKeyBindings();
            bindings.UnbindAll();
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    public class Cmd_bindlist : CCommand
    {
        public Cmd_bindlist()
            : base("bindlist")
        {
        }

        public override void Execute()
        {
            if (ArgsCount() != 0)
            {
                Print("usage: " + name);
                return;
            }

            CKeyBindings bindings = GetRootController().GetKeyBindings();
            List<CKeyBinding> list = bindings.ListBindings();
            for (int i = 0; i < list.Count; ++i)
            {
                String cmd = list[i].cmd;
                String name = list[i].name;
                Print(name + " \"" + cmd + "\"");
            }
        }
    }

    public class Cmd_ctoggle : CCommand
    {
        public Cmd_ctoggle()
            : base("ctoggle")
        {
        }

        public override void Execute()
        {
            if (ArgsCount() != 1)
            {
                Print("usage: " + name + " <cvar>");
                return;
            }

            String cvarName = StrArg(0);
            CVarCommand cmd = console.FindCvarCommand(cvarName);
            if (cmd == null)
            {
                Print("Can't find cvar '" + cvarName + "'");
                return;
            }

            if (!cmd.IsInt())
            {
                Print("Can't toggle non-int value");
                return;
            }

            cmd.SetValue(cmd.boolValue ? 0 : 1);
        }
    }

    //////////////////////////////////////////////////////////////////////////////

    public class CVars
    {
        public static readonly CVar g_drawViewBorders = new CVar("g_drawViewBorders", 0, CFlags.Debug);
        public static readonly CVar d_demoTargetFrame = new CVar("d_demoTargetFrame", 0, CFlags.Debug);
    }
}
