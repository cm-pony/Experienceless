using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Keys = System.Windows.Forms.Keys;
namespace Experienceless.Model
{
    public class HotKey
    {

        public Key Key { get; }

        public ModifierKeys Modifiers { get; }

        public HotKey(Key key, ModifierKeys modifiers)
        {
            Key = key;
            Modifiers = modifiers;
        }

        public static HotKey FromVirtual(Keys[] keys)
        {
            Key key = 0;
            ModifierKeys modifiers = 0;
            for (int i = 0; i < keys.Length; i++)
            {
                Key mkey = KeyInterop.KeyFromVirtualKey((int)keys[i]);
                if (mkey != Key.None)
                {
                    ModifierKeys _modifiers = getModifierKeyFromKey(mkey);
                    if (_modifiers != ModifierKeys.None)
                    {
                        modifiers = modifiers ^ _modifiers;
                    }
                    else {
                        key = mkey;
                    }
                }


            }
            
            return new HotKey(key, modifiers);
        }

        public Keys[] ToVirtual()
        {
            List<Keys> result = new List<Keys>();
            if (Modifiers.HasFlag(ModifierKeys.Control))
                result.Add(Keys.ControlKey);
            if (Modifiers.HasFlag(ModifierKeys.Shift))
                result.Add(Keys.ShiftKey);
            if (Modifiers.HasFlag(ModifierKeys.Alt))
                result.Add(Keys.Menu);
            if (Modifiers.HasFlag(ModifierKeys.Windows))
                result.Add(Keys.LWin);
            result.Add((Keys)KeyInterop.VirtualKeyFromKey(Key));
            while(result.Count < 4)
            {
                result.Add(Keys.None);
            }

            return result.ToArray();
        }

        public override string ToString()
        {
            var str = new StringBuilder();

            if (Modifiers.HasFlag(ModifierKeys.Control))
                str.Append("Ctrl + ");
            if (Modifiers.HasFlag(ModifierKeys.Shift))
                str.Append("Shift + ");
            if (Modifiers.HasFlag(ModifierKeys.Alt))
                str.Append("Alt + ");
            if (Modifiers.HasFlag(ModifierKeys.Windows))
                str.Append("Win + ");

            str.Append(Key);

            return str.ToString();
        }

        private static ModifierKeys getModifierKeyFromKey(Key key) {
            if (key == Key.LeftCtrl || key == Key.RightCtrl)
            {
                return ModifierKeys.Control;
            }
            if (key == Key.LeftAlt || key == Key.RightAlt)
            {
                return ModifierKeys.Alt;
            }
            if (key == Key.LeftShift || key == Key.RightShift)
            {
                return ModifierKeys.Shift;
            }
            if (key == Key.LWin || key == Key.RWin)
            {
                return ModifierKeys.Windows;
            }

            return ModifierKeys.None;
        }
        
    }
}
