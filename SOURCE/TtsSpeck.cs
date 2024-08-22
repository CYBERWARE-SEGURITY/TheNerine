using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace MandelbrotFractal
{
    public class TtsSpeck
    {
        public static void SpeckCyberware()
        {
            SpeechSynthesizer synthesizer = new SpeechSynthesizer();

            synthesizer.SelectVoiceByHints(VoiceGender.Male, VoiceAge.Adult, 0, new System.Globalization.CultureInfo("pt-BR"));

            synthesizer.Rate = 3;

            synthesizer.Volume = 100; 

            PromptBuilder buld = new PromptBuilder();

            while(true)
            {
                buld.AppendText("7 7 7 7 HAHA 777 7 7 7 HAHA!!");

                // Fala o texto
                synthesizer.Speak(buld);
            }
        }

    }
}
