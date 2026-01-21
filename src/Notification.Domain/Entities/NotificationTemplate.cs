using Notification.Domain.Common;
using Notification.Domain.Enums;

namespace Notification.Domain.Entities;

public class NotificationTemplate : AggregateRoot<Guid>
{
    public string Name { get; private set; } = null!;
    public string SubjectTemplate { get; private set; } = null!;
    public string BodyTemplate { get; private set; } = null!;
    public ChannelType Channel { get; private set; }
    public bool IsHtml { get; private set; }
    public bool IsActive { get; private set; }
    public List<string> RequiredVariables { get; private set; } = [];

    private NotificationTemplate() { }

    private NotificationTemplate(
        Guid id,
        string name,
        string subjectTemplate,
        string bodyTemplate,
        ChannelType channel,
        bool isHtml) : base(id)
    {
        Name = name;
        SubjectTemplate = subjectTemplate;
        BodyTemplate = bodyTemplate;
        Channel = channel;
        IsHtml = isHtml;
        IsActive = true;
        RequiredVariables = ExtractVariables(subjectTemplate + bodyTemplate);
    }

    public static NotificationTemplate Create(
        string name,
        string subjectTemplate,
        string bodyTemplate,
        ChannelType channel,
        bool isHtml = false)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Template name cannot be empty.", nameof(name));

        if (string.IsNullOrWhiteSpace(bodyTemplate))
            throw new ArgumentException("Body template cannot be empty.", nameof(bodyTemplate));

        return new NotificationTemplate(
            Guid.NewGuid(),
            name.Trim(),
            subjectTemplate?.Trim() ?? string.Empty,
            bodyTemplate.Trim(),
            channel,
            isHtml);
    }

    public void Update(string name, string subjectTemplate, string bodyTemplate, bool isHtml)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Template name cannot be empty.", nameof(name));

        if (string.IsNullOrWhiteSpace(bodyTemplate))
            throw new ArgumentException("Body template cannot be empty.", nameof(bodyTemplate));

        Name = name.Trim();
        SubjectTemplate = subjectTemplate?.Trim() ?? string.Empty;
        BodyTemplate = bodyTemplate.Trim();
        IsHtml = isHtml;
        RequiredVariables = ExtractVariables(subjectTemplate + bodyTemplate);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public string RenderSubject(Dictionary<string, string> variables)
    {
        return RenderTemplate(SubjectTemplate, variables);
    }

    public string RenderBody(Dictionary<string, string> variables)
    {
        return RenderTemplate(BodyTemplate, variables);
    }

    public bool ValidateVariables(Dictionary<string, string> variables)
    {
        return RequiredVariables.All(v => variables.ContainsKey(v));
    }

    private static string RenderTemplate(string template, Dictionary<string, string> variables)
    {
        var result = template;
        foreach (var (key, value) in variables)
        {
            result = result.Replace($"{{{{{key}}}}}", value);
        }
        return result;
    }

    private static List<string> ExtractVariables(string template)
    {
        var variables = new List<string>();
        var startIndex = 0;

        while (true)
        {
            var openBrace = template.IndexOf("{{", startIndex, StringComparison.Ordinal);
            if (openBrace == -1) break;

            var closeBrace = template.IndexOf("}}", openBrace, StringComparison.Ordinal);
            if (closeBrace == -1) break;

            var variableName = template.Substring(openBrace + 2, closeBrace - openBrace - 2).Trim();
            if (!string.IsNullOrWhiteSpace(variableName) && !variables.Contains(variableName))
            {
                variables.Add(variableName);
            }

            startIndex = closeBrace + 2;
        }

        return variables;
    }
}
