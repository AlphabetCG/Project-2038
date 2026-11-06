# Rebuild the diagram from graph data; no external packages required on Windows.
# Codes are blank here and filled by CodeTexture from PuzzleCodes at runtime.
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$graphs = @(
    @{ Name='CPE'; Center=365; Top=320; Nodes=@(@(0,0),@(1,.08),@(0,.42),@(1,.5),@(.2,.82),@(.62,1),@(1,.85)); Edges='12 13 14 23 24 34 35 37 47 56 67' },
    @{ Name='INC'; Center=1055; Top=320; Nodes=@(@(0,.18),@(1,.25),@(0,.72),@(1,.8),@(.5,.5),@(.5,0),@(.5,1)); Edges='16 26 12 13 15 25 24 35 45 34 37 47' },
    @{ Name='ENE'; Center=1745; Top=320; Nodes=@(@(.08,.2),@(.45,0),@(.85,.1),@(1,.45),@(.9,.82),@(.5,1),@(0,.75)); Edges='12 23 34 45 56 67 17' },
    @{ Name='ME'; Center=2435; Top=320; Nodes=@(@(1,.12),@(1,.5),@(1,.88),@(0,0),@(0,.33),@(0,.67),@(0,1)); Edges='14 15 16 17 24 25 26 27 34 35 36 37' },
    @{ Name='APE'; Center=710; Top=1100; Nodes=@(@(0,.2),@(.5,0),@(1,.2),@(.3,.55),@(.65,.55),@(.05,1),@(1,1)); Edges='12 23 37 67 16 24 45 35' },
    @{ Name='ENV'; Center=1400; Top=1100; Nodes=@(@(0,.25),@(.5,0),@(1,.15),@(.9,1),@(.5,1),@(.05,1),@(.32,1)); Edges='12 16 17 26 23 24 34 35 45 57' },
    @{ Name='PE'; Center=2090; Top=1100; Nodes=@(@(0,.25),@(.5,0),@(1,.15),@(1,.75),@(.75,1),@(.25,1),@(0,.75)); Edges='13 24 27 56' }
)

# Confirm that APE alone has an open Euler trail (connected, two odd vertices).
$solutions = @()
foreach ($graph in $graphs) {
    $degree = @(0,0,0,0,0,0,0)
    $adj = @(@(),@(),@(),@(),@(),@(),@())
    foreach ($edge in $graph.Edges.Split(' ')) {
        $a = [int]::Parse($edge.Substring(0,1)) - 1
        $b = [int]::Parse($edge.Substring(1,1)) - 1
        $degree[$a]++; $degree[$b]++
        $adj[$a] += $b; $adj[$b] += $a
    }
    $seen = [System.Collections.Generic.HashSet[int]]::new()
    $stack = [System.Collections.Generic.Stack[int]]::new()
    $stack.Push(0)
    while ($stack.Count -gt 0) {
        $v = $stack.Pop()
        if ($seen.Add($v)) { foreach ($n in $adj[$v]) { $stack.Push($n) } }
    }
    if ($seen.Count -eq 7 -and @($degree | Where-Object { $_ % 2 -eq 1 }).Count -eq 2) { $solutions += $graph.Name }
}
if ($solutions.Count -ne 1 -or $solutions[0] -ne 'APE') { throw "Ambiguous Euler map: $solutions" }

$bitmap = [System.Drawing.Bitmap]::new(2800,2000)
$canvas = [System.Drawing.Graphics]::FromImage($bitmap)
$canvas.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
$canvas.TextRenderingHint = [System.Drawing.Text.TextRenderingHint]::AntiAliasGridFit
$ink = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(28,28,28))
$paper = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(248,245,234))
$line = [System.Drawing.Pen]::new($ink,5)
$border = [System.Drawing.Pen]::new([System.Drawing.Color]::FromArgb(170,162,143),3)
$title = [System.Drawing.Font]::new('Consolas',78,[System.Drawing.FontStyle]::Bold,[System.Drawing.GraphicsUnit]::Pixel)
$body = [System.Drawing.Font]::new('Consolas',38,[System.Drawing.FontStyle]::Regular,[System.Drawing.GraphicsUnit]::Pixel)
$label = [System.Drawing.Font]::new('Consolas',56,[System.Drawing.FontStyle]::Bold,[System.Drawing.GraphicsUnit]::Pixel)
$number = [System.Drawing.Font]::new('Consolas',38,[System.Drawing.FontStyle]::Bold,[System.Drawing.GraphicsUnit]::Pixel)
$format = [System.Drawing.StringFormat]::new()
$format.Alignment = [System.Drawing.StringAlignment]::Center
$format.LineAlignment = [System.Drawing.StringAlignment]::Center
function Text-Centered([string]$text, [single]$x, [single]$y, $font) {
    $canvas.DrawString($text,$font,$ink,[System.Drawing.PointF]::new($x,$y),$format)
}
try {
    $canvas.Clear($paper.Color)
    Text-Centered "KONIGSMONGKUTT'S MAP" 1400 95 $title
    Text-Centered 'Find the graph you can trace using every edge exactly once.' 1400 195 $body
    Text-Centered 'Start and finish at DIFFERENT vertices. Use its code to log in.' 1400 250 $body
    foreach ($graph in $graphs) {
        $left = $graph.Center - 320
        $canvas.DrawRectangle($border,$left,$graph.Top,640,720)
        Text-Centered $graph.Name $graph.Center ($graph.Top+55) $label
        $points = @()
        foreach ($node in $graph.Nodes) {
            $points += [System.Drawing.PointF]::new(($left+80+$node[0]*480),($graph.Top+145+$node[1]*400))
        }
        foreach ($edge in $graph.Edges.Split(' ')) {
            $a = [int]::Parse($edge.Substring(0,1)) - 1
            $b = [int]::Parse($edge.Substring(1,1)) - 1
            $canvas.DrawLine($line,$points[$a],$points[$b])
        }
        for ($i=0; $i -lt 7; $i++) {
            $p = $points[$i]
            $canvas.FillEllipse($paper,($p.X-30),($p.Y-30),60,60)
            $canvas.DrawEllipse($line,($p.X-30),($p.Y-30),60,60)
            Text-Centered ($i+1).ToString() $p.X $p.Y $number
        }
        # The runtime code is centered at Top + 640; reserve a generous blank strip.
        $canvas.DrawLine($border,($left+35),($graph.Top+585),($left+605),($graph.Top+585))
    }
    Text-Centered 'Crossing lines are NOT vertices unless a numbered circle is shown.' 1400 1910 $body
    $output = Join-Path $PSScriptRoot '../Assets/Pizzle/EulerMap_Blank.png'
    $bitmap.Save([System.IO.Path]::GetFullPath($output),[System.Drawing.Imaging.ImageFormat]::Png)
    Write-Output 'Generated EulerMap_Blank.png; verified APE is the only open Euler trail.'
} finally {
    foreach ($resource in @($format,$number,$label,$body,$title,$border,$line,$paper,$ink,$canvas,$bitmap)) { $resource.Dispose() }
}
