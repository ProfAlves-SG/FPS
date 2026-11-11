# Generates humanoid AnimationClip (.anim) assets for the boss.
# Curve indices and the standing baseline were measured from this project's own
# imported clips (HumanM@MilitaryIdle01 / Walk01_Forward) and from Unity's
# HumanoidDefault.anim, so bindings and sign conventions are not guesses.

param([string]$OutDir = "C:\Users\hp\Desktop\DeleteAfter\Teste\FPS\Assets\AnimationsPackage\_BossSetup")

# --- canonical humanoid curve index table (from HumanoidDefault.anim, indices 7..136)
$IDX = [ordered]@{
  'RootT.x' = 7; 'RootT.y' = 8; 'RootT.z' = 9
  'RootQ.x' = 10; 'RootQ.y' = 11; 'RootQ.z' = 12; 'RootQ.w' = 13
  'Spine Front-Back' = 42; 'Spine Left-Right' = 43
  'Chest Front-Back' = 45
  'Neck Nod Down-Up' = 51
  'Head Nod Down-Up' = 54
  'Left Upper Leg Front-Back' = 63; 'Left Upper Leg In-Out' = 64
  'Left Lower Leg Stretch' = 66
  'Left Foot Up-Down' = 68
  'Right Upper Leg Front-Back' = 71; 'Right Upper Leg In-Out' = 72
  'Right Lower Leg Stretch' = 74
  'Right Foot Up-Down' = 76
  'Left Shoulder Down-Up' = 79
  'Left Arm Down-Up' = 81; 'Left Arm Front-Back' = 82
  'Left Forearm Stretch' = 84
  'Right Shoulder Down-Up' = 88
  'Right Arm Down-Up' = 90; 'Right Arm Front-Back' = 91
  'Right Forearm Stretch' = 93
}

# --- measured standing baseline (MilitaryIdle01), legs symmetrised for a boss stance
$BASE = @{
  'RootT.x' = 0.0;   'RootT.y' = 0.94;  'RootT.z' = 0.05
  'RootQ.x' = 0.048; 'RootQ.y' = 0.0;   'RootQ.z' = 0.0; 'RootQ.w' = 0.9988
  'Spine Front-Back' = -0.21; 'Spine Left-Right' = 0.0
  'Chest Front-Back' = -0.20
  'Neck Nod Down-Up' = -0.29
  'Head Nod Down-Up' = 0.47
  'LUpperFB' = 0.25; 'LUpperIO' = 0.18; 'LLower' = 0.65; 'LFoot' = -0.05
  'RUpperFB' = 0.25; 'RUpperIO' = 0.18; 'RLower' = 0.65; 'RFoot' = -0.05
  'LShoulder' = -0.21; 'LArmDU' = -0.45; 'LArmFB' = 0.34; 'LFore' = 0.40
  'RShoulder' = 0.05;  'RArmDU' = -0.45; 'RArmFB' = 0.34; 'RFore' = 0.40
}

# map the short mirrored keys onto the real curve names
$MIRROR = @{
  'LUpperFB' = 'Left Upper Leg Front-Back'; 'RUpperFB' = 'Right Upper Leg Front-Back'
  'LUpperIO' = 'Left Upper Leg In-Out';     'RUpperIO' = 'Right Upper Leg In-Out'
  'LLower'   = 'Left Lower Leg Stretch';    'RLower'   = 'Right Lower Leg Stretch'
  'LFoot'    = 'Left Foot Up-Down';         'RFoot'    = 'Right Foot Up-Down'
  'LShoulder'= 'Left Shoulder Down-Up';     'RShoulder'= 'Right Shoulder Down-Up'
  'LArmDU'   = 'Left Arm Down-Up';          'RArmDU'   = 'Right Arm Down-Up'
  'LArmFB'   = 'Left Arm Front-Back';       'RArmFB'   = 'Right Arm Front-Back'
  'LFore'    = 'Left Forearm Stretch';      'RFore'    = 'Right Forearm Stretch'
}

# helper: a pose = baseline + overrides. 'legs'/'arms' set both sides at once.
function Pose([hashtable]$o) {
  $p = @{}
  foreach ($k in $BASE.Keys) { $p[$k] = $BASE[$k] }
  foreach ($k in $o.Keys) {
    switch ($k) {
      'upperFB'  { $p['LUpperFB'] = $o[$k]; $p['RUpperFB'] = $o[$k] }
      'upperIO'  { $p['LUpperIO'] = $o[$k]; $p['RUpperIO'] = $o[$k] }
      'lower'    { $p['LLower']   = $o[$k]; $p['RLower']   = $o[$k] }
      'foot'     { $p['LFoot']    = $o[$k]; $p['RFoot']    = $o[$k] }
      'shoulder' { $p['LShoulder'] = $BASE['LShoulder'] + $o[$k]; $p['RShoulder'] = $BASE['RShoulder'] + $o[$k] }
      'armDU'    { $p['LArmDU']   = $o[$k]; $p['RArmDU']   = $o[$k] }
      'armFB'    { $p['LArmFB']   = $o[$k]; $p['RArmFB']   = $o[$k] }
      'fore'     { $p['LFore']    = $o[$k]; $p['RFore']    = $o[$k] }
      default    { $p[$k] = $o[$k] }
    }
  }
  return $p
}

# ---------------- the four clips ----------------
$clips = @()

# 1. PREPARE JUMP - coil down, arms swing back
$clips += @{
  Name = 'Boss_PrepareJump'; Loop = 0
  Keys = @(
    @{ t = 0.0;  p = Pose @{} },
    @{ t = 0.30; p = Pose @{ 'RootT.y' = 0.62; upperFB = -0.75; upperIO = 0.35; lower = -0.35; foot = -0.55
                             'Spine Front-Back' = -0.60; 'Chest Front-Back' = -0.45
                             'Neck Nod Down-Up' = -0.10; 'Head Nod Down-Up' = 0.60
                             armDU = -0.80; armFB = 0.80; fore = 0.75 } },
    @{ t = 0.50; p = Pose @{ 'RootT.y' = 0.66; upperFB = -0.70; upperIO = 0.33; lower = -0.28; foot = -0.50
                             'Spine Front-Back' = -0.55; 'Chest Front-Back' = -0.42
                             'Neck Nod Down-Up' = -0.08; 'Head Nod Down-Up' = 0.62
                             armDU = -0.78; armFB = 0.75; fore = 0.72 } }
  )
}

# 2. JUMP - airborne, legs tucked, arms raised for the slam. Loops while in the air.
$clips += @{
  Name = 'Boss_Jump'; Loop = 1
  Keys = @(
    @{ t = 0.0;  p = Pose @{ 'RootT.y' = 1.02; upperFB = -0.55; upperIO = 0.25; lower = 0.10; foot = 0.45
                             'Spine Front-Back' = -0.30; 'Chest Front-Back' = -0.15
                             'Neck Nod Down-Up' = -0.35; 'Head Nod Down-Up' = 0.35
                             armDU = 0.75; armFB = -0.25; fore = 0.30; shoulder = 0.25 } },
    @{ t = 0.30; p = Pose @{ 'RootT.y' = 1.05; upperFB = -0.62; upperIO = 0.28; lower = 0.02; foot = 0.52
                             'Spine Front-Back' = -0.22; 'Chest Front-Back' = -0.10
                             'Neck Nod Down-Up' = -0.38; 'Head Nod Down-Up' = 0.30
                             armDU = 0.88; armFB = -0.18; fore = 0.22; shoulder = 0.32 } },
    @{ t = 0.60; p = Pose @{ 'RootT.y' = 1.02; upperFB = -0.55; upperIO = 0.25; lower = 0.10; foot = 0.45
                             'Spine Front-Back' = -0.30; 'Chest Front-Back' = -0.15
                             'Neck Nod Down-Up' = -0.35; 'Head Nod Down-Up' = 0.35
                             armDU = 0.75; armFB = -0.25; fore = 0.30; shoulder = 0.25 } }
  )
}

# 3. LAND - slam into the ground, deep squash, then recover to standing
$clips += @{
  Name = 'Boss_Land'; Loop = 0
  Keys = @(
    @{ t = 0.0;  p = Pose @{ 'RootT.y' = 1.00; upperFB = -0.10; upperIO = 0.22; lower = 0.70; foot = 0.20
                             'Spine Front-Back' = -0.25
                             armDU = 0.60; armFB = -0.30; fore = 0.45; shoulder = 0.20 } },
    @{ t = 0.10; p = Pose @{ 'RootT.y' = 0.52; upperFB = -0.85; upperIO = 0.45; lower = -0.50; foot = -0.65
                             'Spine Front-Back' = -0.70; 'Chest Front-Back' = -0.50
                             'Neck Nod Down-Up' = -0.45; 'Head Nod Down-Up' = 0.20
                             armDU = -0.90; armFB = -0.55; fore = 0.80 } },
    @{ t = 0.32; p = Pose @{ 'RootT.y' = 0.72; upperFB = -0.60; upperIO = 0.36; lower = -0.10; foot = -0.40
                             'Spine Front-Back' = -0.52; 'Chest Front-Back' = -0.38
                             'Neck Nod Down-Up' = -0.30; 'Head Nod Down-Up' = 0.32
                             armDU = -0.78; armFB = -0.30; fore = 0.65 } },
    @{ t = 0.80; p = Pose @{} }
  )
}

# 4. SPAWN - summon gesture: both arms sweep up and out, chest opens
$clips += @{
  Name = 'Boss_Spawn'; Loop = 0
  Keys = @(
    @{ t = 0.0;  p = Pose @{} },
    @{ t = 0.35; p = Pose @{ 'RootT.y' = 0.97; upperFB = 0.15; upperIO = 0.25; lower = 0.80; foot = 0.35
                             'Spine Front-Back' = 0.25; 'Chest Front-Back' = 0.20
                             'Neck Nod Down-Up' = 0.10; 'Head Nod Down-Up' = 0.70
                             armDU = 0.90; armFB = -0.10; fore = 0.65; shoulder = 0.30 } },
    @{ t = 0.62; p = Pose @{ 'RootT.y' = 0.96; upperFB = 0.17; upperIO = 0.24; lower = 0.78; foot = 0.30
                             'Spine Front-Back' = 0.22; 'Chest Front-Back' = 0.18
                             'Neck Nod Down-Up' = 0.08; 'Head Nod Down-Up' = 0.68
                             armDU = 0.85; armFB = -0.12; fore = 0.62; shoulder = 0.28 } },
    @{ t = 1.00; p = Pose @{} }
  )
}

# ---------------- emit ----------------
function CurveName([string]$k) { if ($MIRROR.ContainsKey($k)) { return $MIRROR[$k] } else { return $k } }

function Fmt([double]$v) { return [string]::Format([System.Globalization.CultureInfo]::InvariantCulture, '{0:0.#######}', $v) }

New-Item -ItemType Directory -Force -Path $OutDir | Out-Null

# fixed, deterministic guids so the controller can reference the clips
$guids = @{
  'Boss_PrepareJump' = 'a1c73e9052b44d7f81e6b0947c35fd21'
  'Boss_Jump'        = 'b2d84fa163c55e8092f7c1a58d46fe32'
  'Boss_Land'        = 'c3e95ab274d66f91a308d2b69e57af43'
  'Boss_Spawn'       = 'd4fa6bc385e77fa2b419e3c7af68bc54'
}

foreach ($clip in $clips) {
  # ordered list of (index, curveName, values-per-key)
  $rows = @()
  foreach ($short in $BASE.Keys) {
    $cn = CurveName $short
    if (-not $IDX.Contains($cn)) { throw "no index for curve '$cn'" }
    $rows += [pscustomobject]@{ Index = [int]$IDX[$cn]; Name = $cn; Short = $short }
  }
  $rows = $rows | Sort-Object Index

  $sb = New-Object System.Text.StringBuilder
  $null = $sb.AppendLine('%YAML 1.1')
  $null = $sb.AppendLine('%TAG !u! tag:unity3d.com,2011:')
  $null = $sb.AppendLine('--- !u!74 &7400000')
  $null = $sb.AppendLine('AnimationClip:')
  $null = $sb.AppendLine('  m_ObjectHideFlags: 0')
  $null = $sb.AppendLine('  m_CorrespondingSourceObject: {fileID: 0}')
  $null = $sb.AppendLine('  m_PrefabInstance: {fileID: 0}')
  $null = $sb.AppendLine('  m_PrefabAsset: {fileID: 0}')
  $null = $sb.AppendLine("  m_Name: $($clip.Name)")
  $null = $sb.AppendLine('  serializedVersion: 6')
  $null = $sb.AppendLine('  m_Legacy: 0')
  $null = $sb.AppendLine('  m_Compressed: 0')
  $null = $sb.AppendLine('  m_UseHighQualityCurve: 1')
  $null = $sb.AppendLine('  m_RotationCurves: []')
  $null = $sb.AppendLine('  m_CompressedRotationCurves: []')
  $null = $sb.AppendLine('  m_EulerCurves: []')
  $null = $sb.AppendLine('  m_PositionCurves: []')
  $null = $sb.AppendLine('  m_ScaleCurves: []')

  # build each curve block once, reuse for m_FloatCurves and m_EditorCurves
  $blocks = @()
  foreach ($r in $rows) {
    $vals = @()
    foreach ($k in $clip.Keys) { $vals += [double]$k.p[$r.Short] }
    $times = @()
    foreach ($k in $clip.Keys) { $times += [double]$k.t }

    $b = New-Object System.Text.StringBuilder
    $null = $b.AppendLine('  - curve:')
    $null = $b.AppendLine('      serializedVersion: 2')
    $null = $b.AppendLine('      m_Curve:')
    for ($i = 0; $i -lt $vals.Count; $i++) {
      # auto (Catmull-Rom) tangents; flat at the ends
      if ($i -eq 0 -or $i -eq $vals.Count - 1) { $slope = 0.0 }
      else {
        $dt = $times[$i+1] - $times[$i-1]
        if ($dt -le 0) { $slope = 0.0 } else { $slope = ($vals[$i+1] - $vals[$i-1]) / $dt }
      }
      $null = $b.AppendLine('      - serializedVersion: 3')
      $null = $b.AppendLine("        time: $(Fmt $times[$i])")
      $null = $b.AppendLine("        value: $(Fmt $vals[$i])")
      $null = $b.AppendLine("        inSlope: $(Fmt $slope)")
      $null = $b.AppendLine("        outSlope: $(Fmt $slope)")
      $null = $b.AppendLine('        tangentMode: 0')
      $null = $b.AppendLine('        weightedMode: 0')
      $null = $b.AppendLine('        inWeight: 0.33333334')
      $null = $b.AppendLine('        outWeight: 0.33333334')
    }
    $null = $b.AppendLine('      m_PreInfinity: 2')
    $null = $b.AppendLine('      m_PostInfinity: 2')
    $null = $b.AppendLine('      m_RotationOrder: 4')
    $null = $b.AppendLine("    attribute: $($r.Name)")
    $null = $b.AppendLine('    path: ')
    $null = $b.AppendLine('    classID: 95')
    $null = $b.AppendLine('    script: {fileID: 0}')
    $blocks += $b.ToString()
  }

  $null = $sb.AppendLine('  m_FloatCurves:')
  foreach ($b in $blocks) { $null = $sb.Append($b) }

  $null = $sb.AppendLine('  m_PPtrCurves: []')
  $null = $sb.AppendLine('  m_SampleRate: 30')
  $null = $sb.AppendLine('  m_WrapMode: 0')
  $null = $sb.AppendLine('  m_Bounds:')
  $null = $sb.AppendLine('    m_Center: {x: 0, y: 0, z: 0}')
  $null = $sb.AppendLine('    m_Extent: {x: 0, y: 0, z: 0}')
  $null = $sb.AppendLine('  m_ClipBindingConstant:')
  $null = $sb.AppendLine('    genericBindings:')
  foreach ($r in $rows) {
    $null = $sb.AppendLine('    - serializedVersion: 2')
    $null = $sb.AppendLine('      path: 0')
    $null = $sb.AppendLine("      attribute: $($r.Index)")
    $null = $sb.AppendLine('      script: {fileID: 0}')
    $null = $sb.AppendLine('      typeID: 95')
    $null = $sb.AppendLine('      customType: 8')
    $null = $sb.AppendLine('      isPPtrCurve: 0')
    $null = $sb.AppendLine('      isIntCurve: 0')
    $null = $sb.AppendLine('      isSerializeReferenceCurve: 0')
  }
  $null = $sb.AppendLine('    pptrCurveMapping: []')

  $stop = Fmt ($clip.Keys[-1].t)
  $null = $sb.AppendLine('  m_AnimationClipSettings:')
  $null = $sb.AppendLine('    serializedVersion: 2')
  $null = $sb.AppendLine('    m_AdditiveReferencePoseClip: {fileID: 0}')
  $null = $sb.AppendLine('    m_AdditiveReferencePoseTime: 0')
  $null = $sb.AppendLine('    m_StartTime: 0')
  $null = $sb.AppendLine("    m_StopTime: $stop")
  $null = $sb.AppendLine('    m_OrientationOffsetY: 0')
  $null = $sb.AppendLine('    m_Level: 0')
  $null = $sb.AppendLine('    m_CycleOffset: 0')
  $null = $sb.AppendLine('    m_HasAdditiveReferencePose: 0')
  $null = $sb.AppendLine("    m_LoopTime: $($clip.Loop)")
  $null = $sb.AppendLine('    m_LoopBlend: 0')
  $null = $sb.AppendLine('    m_LoopBlendOrientation: 0')
  $null = $sb.AppendLine('    m_LoopBlendPositionY: 0')
  $null = $sb.AppendLine('    m_LoopBlendPositionXZ: 0')
  $null = $sb.AppendLine('    m_KeepOriginalOrientation: 1')
  $null = $sb.AppendLine('    m_KeepOriginalPositionY: 1')
  $null = $sb.AppendLine('    m_KeepOriginalPositionXZ: 0')
  $null = $sb.AppendLine('    m_HeightFromFeet: 0')
  $null = $sb.AppendLine('    m_Mirror: 0')

  $null = $sb.AppendLine('  m_EditorCurves:')
  foreach ($b in $blocks) { $null = $sb.Append($b) }

  $null = $sb.AppendLine('  m_EulerEditorCurves: []')
  $null = $sb.AppendLine('  m_HasGenericRootTransform: 0')
  $null = $sb.AppendLine('  m_HasMotionFloatCurves: 0')
  $null = $sb.AppendLine('  m_GenerateMotionCurves: 0')
  $null = $sb.AppendLine('  m_Events: []')

  $path = Join-Path $OutDir "$($clip.Name).anim"
  [System.IO.File]::WriteAllText($path, $sb.ToString(), (New-Object System.Text.UTF8Encoding $false))

  $meta = @"
fileFormatVersion: 2
guid: $($guids[$clip.Name])
NativeFormatImporter:
  externalObjects: {}
  mainObjectFileID: 7400000
  userData:
  assetBundleName:
  assetBundleVariant:
"@
  # the trailing newline matters: without it Unity's YAML parser fails on the last key
  [System.IO.File]::WriteAllText("$path.meta", $meta + "`n", (New-Object System.Text.UTF8Encoding $false))

  Write-Output ("{0,-20} curves={1}  keys={2}  dur={3}s  loop={4}  -> {5} lines" -f $clip.Name, $rows.Count, $clip.Keys.Count, $stop, $clip.Loop, ($sb.ToString() -split "`n").Count)
}
