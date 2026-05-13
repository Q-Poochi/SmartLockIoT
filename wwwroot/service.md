---
name: Cyber-Premium Smart Home
colors:
  surface: '#121318'
  surface-dim: '#121318'
  surface-bright: '#38393f'
  surface-container-lowest: '#0d0e13'
  surface-container-low: '#1a1b21'
  surface-container: '#1e1f25'
  surface-container-high: '#292a2f'
  surface-container-highest: '#34343a'
  on-surface: '#e3e1e9'
  on-surface-variant: '#b9cacb'
  inverse-surface: '#e3e1e9'
  inverse-on-surface: '#2f3036'
  outline: '#849495'
  outline-variant: '#3a494b'
  surface-tint: '#00dce6'
  primary: '#e3fdff'
  on-primary: '#00373a'
  primary-container: '#00f3ff'
  on-primary-container: '#006b71'
  inverse-primary: '#00696f'
  secondary: '#d7ffc5'
  on-secondary: '#053900'
  secondary-container: '#2ff801'
  on-secondary-container: '#0f6d00'
  tertiary: '#fdf6ff'
  on-tertiary: '#3c0090'
  tertiary-container: '#e4d5ff'
  on-tertiary-container: '#741bff'
  error: '#ffb4ab'
  on-error: '#690005'
  error-container: '#93000a'
  on-error-container: '#ffdad6'
  primary-fixed: '#6ff6ff'
  primary-fixed-dim: '#00dce6'
  on-primary-fixed: '#002022'
  on-primary-fixed-variant: '#004f53'
  secondary-fixed: '#79ff5b'
  secondary-fixed-dim: '#2ae500'
  on-secondary-fixed: '#022100'
  on-secondary-fixed-variant: '#095300'
  tertiary-fixed: '#e9ddff'
  tertiary-fixed-dim: '#d1bcff'
  on-tertiary-fixed: '#23005b'
  on-tertiary-fixed-variant: '#5700c9'
  background: '#121318'
  on-background: '#e3e1e9'
  surface-variant: '#34343a'
typography:
  headline-lg:
    fontFamily: Sora
    fontSize: 32px
    fontWeight: '700'
    lineHeight: 40px
    letterSpacing: -0.02em
  headline-md:
    fontFamily: Sora
    fontSize: 24px
    fontWeight: '600'
    lineHeight: 32px
    letterSpacing: -0.01em
  body-lg:
    fontFamily: Sora
    fontSize: 16px
    fontWeight: '400'
    lineHeight: 24px
    letterSpacing: '0'
  body-sm:
    fontFamily: Sora
    fontSize: 14px
    fontWeight: '400'
    lineHeight: 20px
    letterSpacing: '0'
  status-mono:
    fontFamily: JetBrains Mono
    fontSize: 12px
    fontWeight: '500'
    lineHeight: 16px
    letterSpacing: 0.05em
  data-display:
    fontFamily: JetBrains Mono
    fontSize: 18px
    fontWeight: '700'
    lineHeight: 24px
    letterSpacing: 0.1em
rounded:
  sm: 0.25rem
  DEFAULT: 0.5rem
  md: 0.75rem
  lg: 1rem
  xl: 1.5rem
  full: 9999px
spacing:
  base: 4px
  xs: 4px
  sm: 8px
  md: 16px
  lg: 24px
  xl: 32px
  margin-mobile: 20px
  gutter-mobile: 12px
---

## Brand & Style

The design system is engineered to evoke a sense of high-fidelity control and futuristic luxury. It targets an audience that values both technical precision and aesthetic sophistication, blending the "high-tech" grit of cyberpunk with "high-life" premium minimalism.

The visual direction is **Cyber-Glassmorphism**. It utilizes a deep, abyssal background to make neon accents feel like active light sources rather than static colors. Surfaces are treated as semi-transparent technical glass, emphasizing depth through optical blurs and micro-fine borders. The emotional response should be one of "effortless command"—a clean, calm interface that houses immense power underneath.

Key stylistic pillars include:
- **Optical Depth:** Layered frosted surfaces that suggest a multi-dimensional UI.
- **Luminescent Accents:** Using light as a functional signifier for "on" states and active telemetry.
- **Information Density:** Clean, generous whitespace for primary navigation, contrasted with dense, technical monospaced readouts for device data.

## Colors

The palette is anchored by an ultra-dark navy-black base to ensure maximum contrast for light-based effects. 

- **Primary (Neon Cyan):** Used for primary actions, system status, and "cool" temperature controls. It represents connectivity and the network.
- **Secondary (Vibrant Green):** Used for power-saving modes, security "armed" states, and "safe" confirmations. 
- **Accents & Glows:** Both primary colors should be used as subtle outer glows (box-shadows with high blur, low opacity) to simulate the bleed of light from physical hardware buttons.
- **Glass Surfaces:** Backgrounds for containers use a highly desaturated, low-opacity white to create the "frosted" effect over the dark base.

## Typography

The typography strategy employs a dual-font system to distinguish between the "User Interface" and the "Machine Data."

- **Sora (UI & Navigation):** This geometric sans-serif provides a sleek, modern feel for all human-centric interaction points. Its wide apertures ensure legibility on mobile screens even at lower brightness.
- **JetBrains Mono (Status & Telemetry):** Reserved exclusively for device readouts (e.g., temperature, voltage, signal strength, logs). This font should often be set in all caps for status labels to reinforce the "terminal" aesthetic.

**Hierarchy Note:** Use high-contrast weights for headlines (700) and lighter weights for body text to maintain the minimalist, airy feel.

## Layout & Spacing

The layout follows a **Fluid Grid** model with generous margins to prevent the interface from feeling "claustrophobic"—a common pitfall of cyberpunk aesthetics.

- **Mobile Rhythm:** A 4-column grid is used for mobile portrait views.
- **Whitespace:** Use `xl` (32px) spacing between major functional groups (e.g., separating Climate Control from Lighting).
- **Alignment:** Elements should align to a strict 4px baseline grid to maintain a disciplined, engineered look.
- **Safe Areas:** Ensure interactive elements are kept within the vertical center of the device to allow for comfortable one-handed "thumb-zone" operation.

## Elevation & Depth

This design system eschews traditional shadows in favor of **Luminous Layers** and **Backdrop Blurs**.

1.  **Base Layer:** The solid #0A0B10 background.
2.  **Mid Layer (Containers):** Semi-transparent cards with a `backdrop-filter: blur(20px)`. These must have a 1px solid border using the `border_glass_hex` to define the edges.
3.  **Top Layer (Active/Floating):** Elements like active buttons or floating action buttons (FABs) utilize a soft outer glow (`box-shadow`) using the primary or secondary color at 20-30% opacity.

The "depth" is created by the intensity of the blur; the more important the information, the higher the background blur behind it.

## Shapes

The design system utilizes **Rounded** geometry (0.5rem base) to soften the "industrial" nature of the cyberpunk aesthetic, making it feel premium and consumer-ready.

- **Buttons & Cards:** Use the 0.5rem (8px) base radius.
- **System Badges:** Small status chips should be fully pill-shaped (rounded-xl) to contrast against the structured cards.
- **Input Fields:** Follow the card radius for consistency.

Avoid sharp 0px corners, as they appear too aggressive for a "home" environment.

## Components

### Buttons
- **Primary:** Solid Cyan (#00F3FF) text on a glass background with a Cyan outer glow. Border is 1.5px Cyan.
- **Ghost:** White text (80% opacity) with a thin glass border. No glow until hovered/pressed.

### Cards (The "Glass Shell")
Cards are the primary container. They feature a `20px` background blur and a `1px` border that is slightly brighter at the top-left to simulate a "rim light" effect.

### Input Fields
Minimalist underlines or subtle glass containers. The cursor and focus state must use the Neon Cyan primary color with a micro-glow.

### Status Chips (The "Telemetry")
Small, pill-shaped indicators using `JetBrains Mono`. An active device is marked by a "pulse" animation—a soft, repeating glow expanding from the chip.

### Controls (Sliders/Dials)
- **Sliders:** The track is a thin, dark glass line. The handle (thumb) is a high-contrast Cyan or Green circle with a significant outer glow to make it appear as if it's "projecting" light onto the track.

### Smart Home Specifics
- **Device Node:** A square glass card with a large icon (24pt) and a monospaced "UPTIME" or "SIGNAL" readout in the corner.
- **Floorplan View:** Vector-based strokes with a "Scanning" animation effect—a horizontal line of Cyan light moving vertically across the map.