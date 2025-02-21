use shaderc::{CompileOptions, Compiler};

pub enum ShaderKind {
    Vertex,
    Fragment,
    Compute,
}

pub struct ShaderCompiler {
    compiler: Compiler,
}

impl ShaderCompiler {
    pub fn new() -> Self {
        Self {
            compiler: Compiler::new().unwrap(),
        }
    }

    pub fn compile(&self, text: &str, kind: ShaderKind) -> Vec<u8> {
        let realkind = match kind {
            ShaderKind::Vertex => shaderc::ShaderKind::Vertex,
            ShaderKind::Fragment => shaderc::ShaderKind::Fragment,
            ShaderKind::Compute => shaderc::ShaderKind::Compute,
        };
        
        let mut options = CompileOptions::new().unwrap();
        options.add_macro_definition("EP", Some("main"));
        
        let bin = self.compiler.compile_into_spirv(text, realkind, "shader.hlsl", "main", Some(&options)).unwrap();
        return bin.as_binary_u8().to_vec();
    }
}