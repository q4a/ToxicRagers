﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using ToxicRagers.Generics;
using ToxicRagers.Helpers;

namespace ToxicRagers.CarmageddonReincarnation.Formats
{
    public class MT2 : Material
    {
        XElement xml;

        protected bool bDoubleSided;
        protected bool bCastsShadows;
        protected bool bFogEnabled;
        protected bool bReceivesShadows;

        protected bool bWalkable;
        protected bool bPanickable;

        protected bool bNeedsWorldLightDir;
        protected bool bNeedsWorldSpaceVertexNormal;
        protected bool bNeedsWorldEyePos;
        protected bool bNeedsWorldVertexPos;
        protected bool bNeedsLightingSpaceVertexNormal;
        protected bool bNeedsVertexColour;

        protected string diffuse;
        protected string substance;

        protected List<TextureCoordSource> textureCoordSources;
        protected List<Sampler> samplers;

        protected Vector3 multiplier;

        public bool DoubleSided
        {
            get { return bDoubleSided; }
            set { bDoubleSided = value; }
        }

        public bool CastsShadows
        {
            get { return bCastsShadows; }
            set { bCastsShadows = value; }
        }

        public bool ReceivesShadows
        {
            get { return bReceivesShadows; }
            set { bReceivesShadows = value; }
        }

        public bool Walkable
        {
            get { return bWalkable; }
            set { bWalkable = value; }
        }

        public bool FogEnabled
        {
            get { return bFogEnabled; }
            set { bFogEnabled = value; }
        }

        public bool NeedsWorldSpaceVertexNormal
        {
            get { return bNeedsWorldSpaceVertexNormal; }
            set { bNeedsWorldSpaceVertexNormal = value; }
        }

        public bool NeedsWorldEyePos
        {
            get { return bNeedsWorldEyePos; }
            set { bNeedsWorldEyePos = value; }
        }

        public bool NeedsWorldVertexPos
        {
            get { return bNeedsWorldVertexPos; }
            set { bNeedsWorldVertexPos = value; }
        }

        public bool NeedsWorldLightDir
        {
            get { return bNeedsWorldLightDir; }
            set { bNeedsWorldLightDir = value; }
        }

        public bool NeedsLightingSpaceVertexNormal
        {
            get { return bNeedsLightingSpaceVertexNormal; }
            set { bNeedsLightingSpaceVertexNormal = value; }
        }

        public bool NeedsVertexColour
        {
            get { return bNeedsVertexColour; }
            set { bNeedsVertexColour = value; }
        }

        public bool Panickable
        {
            get { return bPanickable; }
            set { bPanickable = value; }
        }

        public string DiffuseColour
        {
            get { return diffuse; }
            set { diffuse = value; }
        }

        public Vector3 Multiplier
        {
            get { return multiplier; }
            set { multiplier = value; }
        }

        public MT2(XElement xml)
        {
            textureCoordSources = new List<TextureCoordSource>();
            samplers = new List<Sampler>();
            this.xml = xml;

            var dblSided = xml.Descendants("DoubleSided").FirstOrDefault();
            var castsShads = xml.Descendants("CastsShadows").FirstOrDefault();
            var recShads = xml.Descendants("ReceivesShadows").FirstOrDefault();
            var fog = xml.Descendants("FogEnabled").FirstOrDefault();
            var walk = xml.Descendants("Walkable").FirstOrDefault();
            var panic = xml.Descendants("Panickable").FirstOrDefault();
            var needWSVN = xml.Descendants("NeedsWorldSpaceVertexNormal").FirstOrDefault();
            var needWEP = xml.Descendants("NeedsWorldEyePos").FirstOrDefault();
            var needWVP = xml.Descendants("NeedsWorldVertexPos").FirstOrDefault();
            var needWLD = xml.Descendants("NeedsWorldLightDir").FirstOrDefault();
            var needLSVN = xml.Descendants("NeedsLightingSpaceVertexNormal").FirstOrDefault();
            var needVC = xml.Descendants("NeedsVertexColour").FirstOrDefault();

            if (dblSided != null) { bDoubleSided = (dblSided.Attribute("Value").Value.ToLower() == "true"); }
            if (castsShads != null) { bCastsShadows = (castsShads.Attribute("Value").Value.ToLower() == "true"); }
            if (recShads != null) { bReceivesShadows = (recShads.Attribute("Value").Value.ToLower() == "true"); }
            if (fog != null) { bFogEnabled = (fog.Attribute("Value").Value.ToLower() == "true"); }
            if (walk != null) { bWalkable = (walk.Attribute("Value").Value.ToLower() == "true"); }
            if (panic != null) { bPanickable = (panic.Attribute("Value").Value.ToLower() == "true"); }
            if (needWSVN != null) { bNeedsWorldSpaceVertexNormal = (needWSVN.Attribute("Value").Value.ToLower() == "true"); }
            if (needWEP != null) { bNeedsWorldEyePos = (needWEP.Attribute("Value").Value.ToLower() == "true"); }
            if (needWVP != null) { bNeedsWorldVertexPos = (needWVP.Attribute("Value").Value.ToLower() == "true"); }
            if (needWLD != null) { bNeedsWorldLightDir = (needWLD.Attribute("Value").Value.ToLower() == "true"); }
            if (needLSVN != null) { bNeedsLightingSpaceVertexNormal = (needLSVN.Attribute("Value").Value.ToLower() == "true"); }
            if (needVC != null) { bNeedsVertexColour = (needLSVN.Attribute("Value").Value.ToLower() == "true"); }

            var mult = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "Multiplier").FirstOrDefault();

            if (mult != null) { multiplier = ReadConstant(mult); }

            foreach (var textureCoordSource in xml.Descendants("TextureCoordSource"))
            {
                textureCoordSources.Add(TextureCoordSource.CreateFromElement(textureCoordSource));
            }

            foreach (var sampler in xml.Descendants("Sampler"))
            {
                samplers.Add(Sampler.CreateFromElement(sampler));
            }

            var sub = xml.Descendants("Substance").FirstOrDefault();

            if (sub != null) { substance = sub.Attribute("Name").Value; }
        }

        public static MT2 Load(string path)
        {
            //Logger.LogToFile(Logger.LogLevel.Info, path);
            MT2 mt2 = new MT2(XElement.Load(path));

            if (mt2.xml.Descendants("BasedOffOf").Count() > 0)
            {
                string basedOffOf = mt2.xml.Descendants("BasedOffOf").Attributes("Name").First().Value;

                try
                {
                    mt2 = (MT2)Activator.CreateInstance(Type.GetType("ToxicRagers.CarmageddonReincarnation.Formats.Materials." + basedOffOf, true, true), mt2.xml);

                    if (basedOffOf.ToLower() == "glow_simple_norm_spec_env_base")
                    {
                        Logger.LogToFile(Logger.LogLevel.Info, path);
                        //Logger.LogToFile(Logger.LogLevel.Info, mt2.ToString());
                    }
                }
                catch
                {
                    return null;
                }
            }

            return mt2;
        }

        public static Vector3 ReadConstant(XElement constant)
        {
            Vector3 v = Vector3.Zero;

            switch (constant.Attribute("Type").Value.ToLower())
            {
                case "float3":
                    string[] s = constant.Attribute("Value").Value.Split(' ');
                    int mult = (s.Length == 3 ? 1 : 0);
                    v.X = s[0 * mult].ToSingle();
                    v.Y = s[1 * mult].ToSingle();
                    v.Z = s[2 * mult].ToSingle();
                    break;

                case "float":
                    v.X = constant.Attribute("Value").Value.ToSingle();
                    v.Y = constant.Attribute("Value").Value.ToSingle();
                    v.Z = constant.Attribute("Value").Value.ToSingle();
                    break;

                default:
                    throw new NotImplementedException(string.Format("Unknown Type: {0}", constant.Attribute("Type").Value));
            }

            return v;
        }
    }

    public class TextureCoordSource
    {
        string alias;
        bool bScrolling;
        Single scrollX;
        Single scrollY;
        Single scrollZ;
        bool bFlipBook;
        int framesX;
        int framesY;
        Single frameRate;
        bool bFlipBookSelect;
        int flipBookSelectFrame;
        bool bWaving;
        Single waveFrequenceU;
        Single waveFrequenceV;
        Single waveAmplitudeU;
        Single waveAmplitudeV;
        int uvStream;

        public static TextureCoordSource CreateFromElement(XElement element)
        {
            TextureCoordSource tcs = new TextureCoordSource();

            foreach (XAttribute attribute in element.Attributes())
            {
                switch (attribute.Name.LocalName)
                {
                    case "Alias":
                        tcs.alias = element.Attribute("Alias").Value;
                        break;

                    case "UVStream":
                        tcs.uvStream = element.Attribute("UVStream").Value.ToInt();
                        break;

                    case "Scrolling":
                        tcs.bScrolling = (element.Attribute("Scrolling").Value.ToLower() == "true");
                        break;

                    case "ScrollX":
                        tcs.scrollX = element.Attribute("ScrollX").Value.ToSingle();
                        break;

                    case "ScrollY":
                        tcs.scrollY = element.Attribute("ScrollY").Value.ToSingle();
                        break;

                    case "ScrollZ":
                        tcs.scrollZ = element.Attribute("ScrollZ").Value.ToSingle();
                        break;

                    case "FlipBook":
                        tcs.bFlipBook = (element.Attribute("FlipBook").Value.ToLower() == "true");
                        break;

                    case "FramesX":
                        tcs.framesX = element.Attribute("FramesX").Value.ToInt();
                        break;

                    case "FramesY":
                        tcs.framesY = element.Attribute("FramesY").Value.ToInt();
                        break;

                    case "FrameRate":
                        tcs.frameRate = element.Attribute("FrameRate").Value.ToSingle();
                        break;

                    case "FlipBookSelect":
                        tcs.bFlipBookSelect = (element.Attribute("FlipBookSelect").Value.ToLower() == "true");
                        break;

                    case "FlipBookSelectFrame":
                        tcs.flipBookSelectFrame = element.Attribute("FlipBookSelectFrame").Value.ToInt();
                        break;

                    case "Waving":
                        tcs.bWaving = (element.Attribute("Waving").Value.ToLower() == "true");
                        break;

                    case "WaveFrequenceU":
                        tcs.waveFrequenceU = element.Attribute("WaveFrequenceU").Value.ToSingle();
                        break;

                    case "WaveFrequenceV":
                        tcs.waveFrequenceV = element.Attribute("WaveFrequenceV").Value.ToSingle();
                        break;

                    case "WaveAmplitudeU":
                        tcs.waveAmplitudeU = element.Attribute("WaveAmplitudeU").Value.ToSingle();
                        break;

                    case "WaveAmplitudeV":
                        tcs.waveAmplitudeV = element.Attribute("WaveAmplitudeV").Value.ToSingle();
                        break;

                    default:
                        throw new NotImplementedException(string.Format("Unknown Attribute: {0}", attribute.Name.LocalName));
                }
            }

            return tcs;
        }
    }

    public class Sampler
    {
        public enum Filter
        {
            Anisotropic,
            Linear,
            Point,
            None
        }

        public enum Address
        {
            Clamp,
            Wrap
        }

        public enum Usage
        {
            DiffuseAlbedo,
            TangentSpaceNormals,
            SpecAlbedo,
            SpecColour,
            SpecMask,
            SpecPower
        }

        string alias;
        string type;
        Filter minFilter;
        int maxAnisotropy;
        Filter mipFilter;
        Filter magFilter;
        int mipLevelBias;
        Address addressU;
        Address addressV;
        Address addressW;
        bool sRGBRead;
        Usage usageRGB;
        Usage usageAlpha;

        public static Sampler CreateFromElement(XElement element)
        {
            Sampler s = new Sampler();

            foreach (XAttribute attribute in element.Attributes())
            {
                switch (attribute.Name.LocalName)
                {
                    case "Alias":
                        s.alias = element.Attribute("Alias").Value;
                        break;

                    case "Type":
                        s.type = element.Attribute("Type").Value;
                        break;

                    case "MinFilter":
                        s.minFilter = element.Attribute("MinFilter").Value.ToEnum<Filter>();
                        break;

                    case "MaxAnisotropy":
                        s.maxAnisotropy = element.Attribute("MaxAnisotropy").Value.ToInt();
                        break;

                    case "MipFilter":
                        s.mipFilter = element.Attribute("MipFilter").Value.ToEnum<Filter>();
                        break;

                    case "MipLevelBias":
                        s.mipLevelBias = element.Attribute("MipLevelBias").Value.ToInt();
                        break;

                    case "MagFilter":
                        s.magFilter = element.Attribute("MagFilter").Value.ToEnum<Filter>();
                        break;

                    case "AddressU":
                        s.addressU = element.Attribute("AddressU").Value.ToEnum<Address>();
                        break;

                    case "AddressV":
                        s.addressV = element.Attribute("AddressV").Value.ToEnum<Address>();
                        break;

                    case "AddressW":
                        s.addressW = element.Attribute("AddressW").Value.ToEnum<Address>();
                        break;

                    case "sRGBRead":
                        s.sRGBRead = (element.Attribute("sRGBRead").Value.ToLower() == "true");
                        break;

                    case "UsageRGB":
                        s.usageRGB = element.Attribute("UsageRGB").Value.ToEnum<Usage>();
                        break;

                    case "UsageAlpha":
                        s.usageAlpha = element.Attribute("UsageAlpha").Value.ToEnum<Usage>();
                        break;

                    default:
                        throw new NotImplementedException(string.Format("Unknown Attribute: {0}", attribute.Name.LocalName));
                }
            }

            return s;
        }
    }
}