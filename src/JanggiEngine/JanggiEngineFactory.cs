#nullable enable
namespace YuJanggi.Engine.JanggiEngine
{
    using JanggiOption;
    public static class JanggiEngineFactory
    {
        public static IJanggiEngine CreateEngine(JanggiOptions options)
            => new JanggiEngine(options);
    }
}
