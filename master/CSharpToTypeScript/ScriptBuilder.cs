using System.Text;

namespace CSharpToTypeScript
{
    public class ScriptBuilder
    {
        private readonly StringBuilder _internalBuilder;

        public string IndentationString { get; set; }

        public Int32 TabSize { get; set; } = 4;

        public int IndentationLevels { get; private set; }

        public ScriptBuilder()
          : this("\t")
        {
        }

        public ScriptBuilder(string indentation)
        {
            this._internalBuilder = new StringBuilder();
            this.IndentationString = indentation;
        }

        public IndentationLevelScope IncreaseIndentation()
        {
            ++this.IndentationLevels;
            return new IndentationLevelScope(this);
        }

        internal void DecreaseIndentation(IndentationLevelScope indentationScope)
        {
            if (indentationScope == null)
                throw new ArgumentNullException(nameof(indentationScope));
            if (this.IndentationLevels <= 0)
                throw new InvalidOperationException("Indentation level is already set to zero.");
            --this.IndentationLevels;
        }

        public void Append(string value) => this._internalBuilder.Append(value);

        public void AppendLine() => this._internalBuilder.AppendLine();

        public void AppendLine(string value) => this._internalBuilder.AppendLine(value);

        public void AppendFormat(string format, params object?[] args) => this._internalBuilder.AppendFormat(format, args);

        public void AppendIndentation() => this._internalBuilder.Append(string.Concat(Enumerable.Repeat(this.IndentationString, this.IndentationLevels)));

        public void AppendIndented(string value)
        {
            this.AppendIndentation();
            this.Append(value);
        }

        public void AppendFormatIndented(string format, params object?[] args)
        {
            this.AppendIndentation();
            this.AppendFormat(format, args);
        }

        public void AppendLineIndented(string value)
        {
            this.AppendIndentation();
            this.AppendLine(value);
        }

        public override string ToString() => this._internalBuilder.ToString();
    }
}
