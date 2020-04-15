using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Experienceless.Model
{
    public struct EncoderProfile
    {
        public object value { get; set; }
        public string text { get; set; }
        public int bitrate { get; set; }
        public EncoderProfile(string text, object value, int bitrate)
        {
            this.text = text;
            this.value = value;
            this.bitrate = bitrate;
        }
    }

}
