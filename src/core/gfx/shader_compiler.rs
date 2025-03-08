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

    pub fn compile(&self, text: &str, kind: ShaderKind) -> Result<Vec<u8>, shaderc::Error> {
        let realkind = match kind {
            ShaderKind::Vertex => shaderc::ShaderKind::Vertex,
            ShaderKind::Fragment => shaderc::ShaderKind::Fragment,
            ShaderKind::Compute => shaderc::ShaderKind::Compute,
        };

        let mut options = CompileOptions::new().unwrap();
        options.set_source_language(shaderc::SourceLanguage::GLSL);
        // options.add_macro_definition("EP", Some("main"));

        let bin = self
            .compiler
            .compile_into_spirv(text, realkind, "shader.glsl", "main", Some(&options))?;
        Ok(bin.as_binary_u8().to_vec())
    }
}

