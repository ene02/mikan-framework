# 🍊 Mikan.Toolkit

**Mikan Toolkit** is a personal library I created to make use of **BASS**, **SDL**, and **Veldrid** easier for my own projects. It’s a toolkit, not a framework, and is designed to help with the specific tasks I need. While it’s not a comprehensive wrapper for all features of these libraries, it includes the parts that I found useful for my work. 

## What Mikan Is (and Isn’t)

Mikan is a toolkit, not a plug-and-play framework, some setup will be needed and stuff still needs initialization and some level of managment.

## How It Works

Mikan offers a set of abstractions that make working with **BASS**, **SDL**, and **Veldrid** easier, especially if you prefer a more object-oriented approach. It manages handlers internally and exposes higher-level methods that are safer and easier to work with than the raw handlers you’d normally use.

Since Mikan is more focused on exposing more used methods, this might cause problems if you needed something i didnt offer directly, making you have to use libraries directly or directly with a binding.

## Libraries Used

- **BASS**: Used for audio processing, with only the necessary features exposed.
- **SDL**: Handles windowing and input, but again, only the relevant parts are included.
- **Veldrid**: Provides rendering capabilities, but only the features I required are wrapped.

## Cross-Platform & Mobile Support

Mikan is technically cross-platform, but I haven’t thoroughly tested it across all platforms. While mobile support is possible, it hasn’t been tested and I haven’t tried it myself.

## Why I Made This

I created Mikan because I wanted to simplify working with **BASS**, **SDL**, and **Veldrid** for my personal projects. It’s not designed to be a universal solution, just a toolkit that works for what I needed. I’ve included documentation in case others find it useful, but it’s very much a personal project that’s meant to save time and make development easier for me.
