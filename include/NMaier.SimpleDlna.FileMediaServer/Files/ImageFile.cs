﻿using System.Runtime.Serialization;

using NMaier.SimpleDlna.Server.Interfaces;
using NMaier.SimpleDlna.Server.Types;

using TagLib;

using File = TagLib.File;

namespace NMaier.SimpleDlna.FileMediaServer;

[Serializable]
internal sealed class ImageFile :
  BaseFile, IMediaImageResource, ISerializable
{
    private string? creator;

    private string? description;

    private bool initialized;

    private string? title;

    private int? width,
      height;

    internal ImageFile(FileServer server, FileInfo aFile, DlnaMime aType)
      : base(server, aFile, aType, DlnaMediaTypes.Image)
    {
    }

    public string MetaCreator
    {
        get
        {
            MaybeInit();
            return creator ?? string.Empty;
        }
    }

    public string MetaDescription
    {
        get
        {
            MaybeInit();
            return description ?? string.Empty;
        }
    }

    public int? MetaHeight
    {
        get
        {
            MaybeInit();
            return height;
        }
    }

    public int? MetaWidth
    {
        get
        {
            MaybeInit();
            return width;
        }
    }

    public override IHeaders Properties
    {
        get
        {
            MaybeInit();
            var rv = base.Properties;
            if (description != null)
            {
                rv.Add("Description", description);
            }
            if (creator != null)
            {
                rv.Add("Creator", creator);
            }
            if (width != null && height != null)
            {
                rv.Add(
                  "Resolution",
                  $"{width.Value}x{height.Value}"
                  );
            }
            return rv;
        }
    }

    public override string Title
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(title))
            {
                return $"{base.Title} — {title}";
            }
            return base.Title;
        }
    }

    public void GetObjectData(SerializationInfo info, StreamingContext ctx)
    {
        if (info == null)
        {
            throw new ArgumentNullException(nameof(info));
        }
        MaybeInit();
        info.AddValue("cr", creator);
        info.AddValue("d", description);
        info.AddValue("t", title);
        info.AddValue("w", width);
        info.AddValue("h", height);
    }

    private void MaybeInit()
    {
        if (initialized)
        {
            return;
        }

        try
        {
            using (var tl = File.Create(new TagLibFileAbstraction(Item)))
            {
                try
                {
                    width = tl.Properties.PhotoWidth;
                    height = tl.Properties.PhotoHeight;
                }
                catch (Exception ex)
                {
                    Debug("Failed to transpose Properties props", ex);
                }

                try
                {
                    var t = ((TagLib.Image.File)tl).ImageTag;
                    title = t.Title;
                    if (string.IsNullOrWhiteSpace(title))
                    {
                        title = null;
                    }
                    description = t.Comment;
                    if (string.IsNullOrWhiteSpace(description))
                    {
                        description = null;
                    }
                    creator = t.Creator;
                    if (string.IsNullOrWhiteSpace(creator))
                    {
                        creator = null;
                    }
                }
                catch (Exception ex)
                {
                    Debug("Failed to transpose Tag props", ex);
                }
            }


            initialized = true;
        }
        catch (CorruptFileException ex)
        {
            Debug(
              "Failed to read meta data via taglib for file " + Item.FullName, ex);
            initialized = true;
        }
        catch (UnsupportedFormatException ex)
        {
            Debug(
              "Failed to read meta data via taglib for file " + Item.FullName, ex);
            initialized = true;
        }
        catch (Exception ex)
        {
            Warn(
              "Unhandled exception reading meta data for file " + Item.FullName,
              ex);
        }
    }
}
