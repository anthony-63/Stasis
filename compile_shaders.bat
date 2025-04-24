for %%s in (
    "basic"
    "textured"
    "three-d"
    "tinted-textured"
) do (
    .\shaders\glslc.exe shaders/src/%%s.vert -o assets/shaders/%%s.vert.spv
    .\shaders\glslc.exe shaders/src/%%s.frag -o assets/shaders/%%s.frag.spv
)