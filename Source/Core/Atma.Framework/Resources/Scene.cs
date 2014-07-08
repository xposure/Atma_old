
namespace Atma.Resources
{
    public class Scene
    {
        private string _file;
        internal Scene(string file)
        {
            _file = file;
        }

        public void load()
        {
            Root.instance.reload();
            Serializers.deserializeScene(_file);
        }
    }
}
