using imbACE.Core.operations;
using imbACE.Services.consolePlugins;
using imbNLP.Project.Operations.Setups;
using imbNLP.Toolkit.Documents.Ranking;
using imbNLP.Toolkit.Documents.Ranking.Core;
using imbNLP.Toolkit.Documents.Ranking.Data;
using imbNLP.Toolkit.Planes;
using imbSCI.Core.files.folders;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace imbNLP.Project.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="imbACE.Services.consolePlugins.instanceLoadSaveExtension{imbNLP.Project.Operations.Setups.SetupProjectionWeightTableConstruction}" />
    public class becProjectionWeightTableExtension : instanceLoadSaveExtension<SetupProjectionWeightTableConstruction>
    {



        private becDocumentRenderingExtension _primaryRendering;

        public becDocumentRenderingExtension primaryRendering
        {
            get { return _primaryRendering; }
        }

        private becDocumentRenderingExtension _secondaryRendering;

        public becDocumentRenderingExtension secondaryRendering
        {
            get { return _secondaryRendering; }
        }




        public becDocumentSelectionExtension secondaryModel
        {
            get { return _secondaryModel; }
            set { _secondaryModel = value; }
        }


        private becDocumentSelectionExtension _secondaryModel;


        public folderNode folder { get; set; }

        public becProjectionWeightTableExtension(folderNode _folder, IAceOperationSetExecutor __parent) : base(_folder, __parent)
        {
            folder = _folder;

            _primaryRendering = new becDocumentRenderingExtension(this, _folder);


            _secondaryRendering = new becDocumentRenderingExtension(this, _folder);


            _secondaryModel = new becDocumentSelectionExtension(_folder, __parent);



        }

        public override void SetSubBinding()
        {
            _primaryRendering.SetBinding(data, nameof(data.primaryRender), true);
            _secondaryRendering.SetBinding(data, nameof(data.secondaryRender), true);
            _secondaryModel.SetBinding(data, nameof(data.secondaryModel), true);
            SetSubBinding();


        }
    }
}