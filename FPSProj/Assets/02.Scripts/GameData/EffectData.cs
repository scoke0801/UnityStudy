using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
using System;
using Unity.VisualScripting;

public class EffectData : BaseData
{
    public EffectClip[] effectClips = new EffectClip[0];

    public string clipPath = "Prefabs/Effects/";
    private string xmlFilePath = "";
    private string xmlfileName = "effectData.xml";
    private string dataPath = "Data/effectData";

    // 저장 키 값
    private const string EFFECT = "effect";
    private const string CLIP = "clip";

    private EffectData() { }

    public void LoadData()
    {
        Debug.Log($"xmlFilePath = {Application.dataPath} + {dataDirectory}");
        xmlFilePath = Application.dataPath + dataDirectory;

        TextAsset asset = (TextAsset)ResourceManager.Load(dataPath);

        if (asset == null || asset.text == null)
        {
            this.AddData("New Effect");
            return;
        }

        using (XmlTextReader reader = new XmlTextReader(new StringReader(asset.text)))
        {
            int currentId = 0;
            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    switch (reader.Name)
                    {
                        case "length":
                            int length = int.Parse(reader.ReadString());
                            names = new string[length];
                            effectClips = new EffectClip[length];
                            break;
                        case "id":
                            currentId = int.Parse(reader.ReadString());
                            effectClips[currentId] = new EffectClip();
                            effectClips[currentId].realId = currentId;
                            break;
                        case "name":
                            names[currentId] = reader.ReadString();
                            break;

                        case "effectType":
                            effectClips[currentId].effectType = (EffectType)Enum.Parse(typeof(EffectType), reader.ReadString());
                            break;
                        case "effectName":
                            effectClips[currentId].effectName = reader.ReadString();
                            break;
                        case "effectPath":
                            effectClips[currentId].effectPath = reader.ReadString();
                            break;
                    }

                }
            }
        }
    }

    public void SaveData()
    {
        using (XmlTextWriter xml = new XmlTextWriter(xmlFilePath + xmlfileName, System.Text.Encoding.Unicode))
        {
            xml.WriteStartDocument();
            xml.WriteStartElement(EFFECT);
            xml.WriteElementString("length", GetDataCount().ToString());

            for (int i = 0; i < names.Length; ++i)
            {
                EffectClip clip = effectClips[i];
                xml.WriteStartElement(CLIP);
                xml.WriteElementString("id", i.ToString());
                xml.WriteElementString("name", names[i]);
                xml.WriteElementString("effectType", clip.effectType.ToString());
                xml.WriteElementString("effectPath", clip.effectPath);
                xml.WriteElementString("effectName", clip.effectName);
                xml.WriteEndElement();
            }

            xml.WriteEndElement();
            xml.WriteEndDocument();
        }
    }

    public override int AddData(string newName)
    {
        if (names == null)
        {
            this.names = new string[] { name };
            this.effectClips = new EffectClip[] { new EffectClip() };
        }
        else
        {
            this.names = ArrayHelper.Add(name, this.names);
            this.effectClips = ArrayHelper.Add(new EffectClip(), this.effectClips);
        }

        return GetDataCount();
    }

    public override void RemoveData(int index)
    {
        names = ArrayHelper.Remove(index, names);
        if(this.names.Length == 0)
        {
            names = null;
        }

        effectClips = ArrayHelper.Remove(index, effectClips);
    }

    public override void Copy(int index)
    {
        names = ArrayHelper.Add(names[index], names);

        effectClips = ArrayHelper.Add(GetCopy(index), effectClips);
    }

    public void ClearData()
    {
        foreach(EffectClip clip in effectClips)
        {
            clip.ReleaseEffect();
        }

        effectClips = null;
        names = null;
    }

    public EffectClip GetCopy(int index)
    {
        if(index < 0 || index >= this.effectClips.Length)
        {
            return null;
        }

        EffectClip original = this.effectClips[index];
        EffectClip clip = new EffectClip();
        clip.effectFullPath = original.effectFullPath;
        clip.effectName = original.effectName;
        clip.effectPath = original.effectPath;
        clip.effectType = original.effectType;
        clip.realId = original.realId;

        return clip;
    }

    public EffectClip GetClip(int index)
    {
        if(index < 0 || index >= this.effectClips.Length)
        {
            return null;
        }

        effectClips[index].PreLoad();
        return effectClips[index];
    }
}
