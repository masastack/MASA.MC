// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Web.Admin.Store;

public class NoticeState
{
    public bool IsRead => !Notices.Any(x => !x.IsRead);

    public List<WebsiteMessageDto> Notices
    {
        get => _notices;
        set
        {
            if (_notices != value)
            {
                _notices = value;
                OnNoticeChanged?.Invoke();
            }
        }
    }

    public delegate Task NoticeChanged();

    public event NoticeChanged? OnNoticeChanged;

    private List<WebsiteMessageDto> _notices = new();

    public void SetNotices(List<WebsiteMessageDto> notices)
    {
        Notices = notices;
    }

    public void SetAllRead()
    {
        var notices = Notices.Select(x =>
        {
            x.IsRead = true;
            return x;
        });
        Notices = notices.ToList();
    }
}
