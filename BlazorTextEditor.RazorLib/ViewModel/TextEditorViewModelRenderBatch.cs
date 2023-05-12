using BlazorTextEditor.RazorLib.Model;
using BlazorTextEditor.RazorLib.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorTextEditor.RazorLib.ViewModel;

public record TextEditorRenderBatch(
    TextEditorModel? Model,
    TextEditorViewModel? ViewModel,
    TextEditorOptions? Options);
