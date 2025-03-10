mod core;
mod scenes;
mod content;

use core::window::Window;
use std::fs;

use scenes::loading::LoadingScene;
use tracing_subscriber::{fmt, layer::SubscriberExt, Registry};

fn main() {
    if fs::exists("stasis.log").unwrap() {
        fs::remove_file("stasis.log").unwrap();
    }
    let file_appender = tracing_appender::rolling::never("./", "stasis.log");
    let (file_out, _guard) = tracing_appender::non_blocking(file_appender);

    let (stdout, _guard) = tracing_appender::non_blocking(std::io::stdout());

    let subscriber = Registry::default()
        .with(fmt::Layer::default().with_writer(stdout))
        .with(fmt::Layer::default().with_writer(file_out).with_ansi(false));
    
    tracing::subscriber::set_global_default(subscriber).unwrap();

    let mut window = Window::new("Stasis", 1280, 720, Some("Assets/Game/Icon.png"), LoadingScene::new());
    window.run();
}
