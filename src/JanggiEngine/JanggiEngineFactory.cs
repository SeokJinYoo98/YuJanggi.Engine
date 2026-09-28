namespace YuJanggi.Engine.JanggiEngine
{
    using JanggiOption;
    public static class JanggiEngineFactory
    {
        public static IReadonlyEngine CreateEngine(JanggiOptions options)
            => new JanggiEngine(options);
    }
}
