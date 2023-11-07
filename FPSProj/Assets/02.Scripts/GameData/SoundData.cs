using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;

public class SoundData : BaseData
{
    public SoundClip[] soundClips = new SoundClip[0];

    private string clipPath = "Sound/";
    private string xmlFilePath = "";
    private string xmlFileName = "soundData.xml";
    private string dataPath = "Data/soundData/";

    private static string SOUND = "sound";
    private static string CLIP = "clip";

    public SoundData() { }

    public void SaveData()
    {
        using (XmlTextWriter xml = new XmlTextWriter(xmlFilePath + xmlFileName,
            System.Text.Encoding.Unicode))
        {
            xml.WriteStartDocument();
            xml.WriteStartElement(SOUND);

            xml.WriteElementString("length", GetDataCount().ToString());
            xml.WriteWhitespace("\n");

            for(int i = 0; i < names.Length; ++i)
            {
                SoundClip clip = soundClips[i];
                xml.WriteStartElement(CLIP);
                xml.WriteElementString("id", i.ToString());
                xml.WriteElementString("name", names[i]);
                xml.WriteElementString("loops", clip.checkTime.Length.ToString());
                xml.WriteElementString("maxvol", clip.maxVolume.ToString());
                xml.WriteElementString("picth", clip.pitch.ToString());
                xml.WriteElementString("dopplerlevel", clip.dopplerLevel.ToString());
                xml.WriteElementString("rolloffmode", clip.rolloffMode.ToString());
                xml.WriteElementString("mindistance", clip.minDistance.ToString());
                xml.WriteElementString("maxdistance", clip.maxVolume.ToString());
                xml.WriteElementString("sparitablend", clip.sparialBlend.ToString());
                if(clip.GetHasLoop() == true)
                {
                    xml.WriteElementString("loop", "true");
                }
                xml.WriteElementString("clippath", clip.clipPath);
                xml.WriteElementString("clipname", clip.clipName);
                xml.WriteElementString("checktimeout", clip.checkTime.Length.ToString());

                string str = "";
                foreach(float t in clip.checkTime)
                {
                    str += t.ToString() + "/";
                }
                xml.WriteElementString("checktime", str);

                xml.WriteElementString("settimecount", clip.setTime.Length.ToString());
                str = "";
                foreach (float t in clip.setTime)
                {
                    str += t.ToString() + "/";
                }
                xml.WriteElementString("settime", str);
                xml.WriteElementString("type", clip.playType.ToString());


                xml.WriteEndElement();

            }
            xml.WriteEndElement();
            xml.WriteEndDocument();
        }
    }
}
