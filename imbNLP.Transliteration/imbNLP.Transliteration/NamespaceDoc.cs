using imbNLP.Transliteration.ruleSet;
using System.Runtime.CompilerServices;

namespace imbNLP.Transliteration
{
    /// <summary>
    /// <para>Provides simple transliteration use</para>
    /// </summary>
    /// <remarks>
    /// <para>To use in your code:</para>
    /// <para>Add <c>using imbNLP.Transliteration;</c> in header of your file</para>
    /// <para>Then call <see cref="transliterationTool.transliterate(string, string, bool)"/> extension on strings you want to transliterate</para>
    /// <para></para>
    /// <list type="bullet">
    /// 	<listheader>
    ///			<term>How to add your transliteration definition?</term>
    ///			<description>Short guide on adding your definition without recompiling the library</description>
    ///		</listheader>
    ///     <item>
    ///			<term>Add <c>resources/transliteration</c> directory at your solution</term>
    ///			<description>Here all custom transliteration rules should be defined as txt files</description>
    ///		</item>
    ///		<item>
    ///			<term>Add your transliteration definition</term>
    ///			<description>Create new text file in resources/transliteration directory named as: [language_id].txt</description>
    ///		</item>
    ///		<item>
    ///			<term>Set solution item</term>
    ///			<description>Set "Content" and "Copy always" in solution item properties of your definition file</description>
    ///		</item>
    ///		<item>
    ///			<term>To learn about definition format</term>
    ///			<description>Check readme.txt and existing example of sr.txt</description>
    ///		</item>
    /// </list>
    /// </remarks>
    /// <![CDATA[   ]]>
    /// <seealso cref="transliterationTool" />
    /// <seealso cref="transliterationPairSet" />
    /// <seealso cref="transliterationPairEntry" />
    [CompilerGenerated]
    internal class NamespaceDoc
    {
    }
}