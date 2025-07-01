UI Design Instructions - Avans Food Waste Prevention App
Overview
Create a modern, clean, and intuitive user interface inspired by iOS design principles. The app should feel
premium, accessible, and focused on sustainability. Think of apps like Too Good To Go, but with Apple's
design philosophy.
Design Philosophy
Minimalism: Less is more - every element should have a purpose
Clarity: Information hierarchy should be crystal clear
Sustainability Focus: Visual elements should reinforce the environmental mission
Accessibility: Design for all users, including those with disabilities
Visual Style Guide
Color Palette
Primary Colors:
Primary Green: #34C759 (iOS system green - represents sustainability)
Secondary Green: #30B04F (darker variant for hover states)
Accent Green: #E8F5E8 (light green for backgrounds)
Neutral Colors:
Background: #F2F2F7 (iOS system background)
Card Background: #FFFFFF (pure white)
Text Primary: #1C1C1E (iOS label color)
Text Secondary: #8E8E93 (iOS secondary label)
Border: #E5E5EA (iOS separator)
Status Colors:
Success: #34C759
Warning: #FF9500
Error: #FF3B30
Info: #007AFF
Typography
Font Stack:
Type Scale:
Hero Title: 32px, font-weight: 700, line-height: 1.2
Page Title: 28px, font-weight: 600, line-height: 1.3
Section Title: 22px, font-weight: 600, line-height: 1.4
Card Title: 18px, font-weight: 600, line-height: 1.4
Body Text: 16px, font-weight: 400, line-height: 1.5
Small Text: 14px, font-weight: 400, line-height: 1.4
Caption: 12px, font-weight: 400, line-height: 1.3
Layout & Spacing
Grid System
Use CSS Grid or Flexbox instead of Bootstrap's grid
Maximum content width: 1200px
Container padding: 20px on mobile, 40px on desktop
Spacing Scale (use CSS custom properties)
Border Radius
Small elements (buttons, tags): 8px
Cards: 12px
Large cards: 16px
Input fields: 10px
css
ffoonntt--ffaammiillyy: --aappppllee--ssyysstteemm, BBlliinnkkMMaaccSSyysstteemmFFoonntt, ''SSeeggooee UUII'', ''RRoobboottoo'', ''HHeellvveettiiccaa NNeeuuee'', AArriiaall, ssaannss--sseerriiff;
css
:root {
----ssppaacciinngg--xxss: 4px;
----ssppaacciinngg--ssmm: 8px;
----ssppaacciinngg--mmdd: 16px;
----ssppaacciinngg--llgg: 24px;
----ssppaacciinngg--xxll: 32px;
----ssppaacciinngg--xxxxll: 48px;
}
Component Design Guidelines
Navigation
Fixed top navigation with blur effect (backdrop-filter: blur(20px))
Height: 64px
Logo on the left, user menu on the right
Clean, minimal design with subtle shadows
Use SF Symbols or Feather Icons for consistency
Cards
Clean white background with subtle shadow: box-shadow: 0 2px 10px rgba(0,0,0,0.1)
12px border radius
16px padding on mobile, 24px on desktop
Hover state: slight elevation increase and border color change
Food package cards should include:
High-quality food image (if available)
Package name (prominent)
Location and pickup time (secondary text)
Price (highlighted in green)
Availability status (clear visual indicator)
Buttons
Primary Button:
Background: #34C759
Text: White, font-weight: 600
Padding: 12px 24px
Border-radius: 8px
Hover: slightly darker background
Active state: subtle inset shadow
Secondary Button:
Background: transparent
Border: 2px solid #34C759
Text: #34C759 , font-weight: 600
Same padding and radius as primary
Danger Button:
Background: #FF3B30
Text: White, font-weight: 600
Forms
Input fields: Clean, minimal border, focus state with green accent
Labels: Above inputs, font-weight: 600
Validation: Inline feedback with appropriate colors
Form groups: Proper spacing and visual hierarchy
Tables
Clean, minimal design without heavy borders
Zebra striping: Subtle alternating row colors
Hover states: Highlight rows on hover
Responsive: Stack on mobile devices
Page-Specific Guidelines
Dashboard/Home Page
Hero section with sustainability stats or user impact
Quick actions prominently displayed
Recent activity in clean card format
Visual hierarchy with proper spacing
Package Listing
Card-based layout in responsive grid
Filter sidebar on desktop, collapsible on mobile
Search bar with iOS-style design
Empty states with helpful messaging
Loading states with skeleton screens
Package Details
Large food image at the top
Information hierarchy: Name → Location → Time → Price → Description
Action buttons prominently placed
Related suggestions at the bottom
User Profile
Clean form layout with proper spacing
Profile picture with upload functionality
Statistics cards showing user impact
Settings organized in groups
CSS Framework Options
Option 1: Custom CSS (Recommended)
Replace Bootstrap with custom CSS using:
CSS Grid for layout
Flexbox for component alignment
CSS Custom Properties for theming
Modern CSS features (clamp, min/max, etc.)
Custom CSS Architecture
Option 2: Alternative Modern Frameworks
If you prefer using a framework that aligns with this design system, consider:
Tailwind CSS (Highly Recommended)
ssttyylleess//
├├──── bbaassee//
││ ├├──── rreesseett..ccssss
││ ├├──── ttyyppooggrraapphhyy..ccssss
││ └└──── vvaarriiaabblleess..ccssss
├├──── ccoommppoonneennttss//
││ ├├──── bbuuttttoonnss..ccssss
││ ├├──── ccaarrddss..ccssss
││ ├├──── ffoorrmmss..ccssss
││ └└──── nnaavviiggaattiioonn..ccssss
├├──── llaayyoouuttss//
││ ├├──── ggrriidd..ccssss
││ └└──── ccoonnttaaiinneerrss..ccssss
└└──── uuttiilliittiieess//
├├──── ssppaacciinngg..ccssss
└└──── ccoolloorrss..ccssss
Utility-first approach allows precise control
Easy to customize colors, spacing, and components
Excellent for iOS-inspired designs
Small bundle size when purged
Example: bg-green-500 rounded-lg shadow-sm px-6 py-3
Bulma
Modern CSS framework without JavaScript dependencies
Clean, minimal design philosophy
Easy to customize with Sass variables
Good component library that can be styled to match iOS aesthetic
Tachyons
Functional CSS approach
Small, focused utility classes
Great performance
Easy to customize for specific design needs
UI Framework Libraries (Choose One):
Headless UI + Tailwind CSS (React/Vue components)
Radix UI + custom styling (excellent accessibility)
Chakra UI (highly customizable)
Mantine (modern, clean components)
Framework Selection Guidelines
Choose based on:
1. Custom CSS: Maximum control, perfect iOS replication, larger development time
2. Tailwind CSS: Best balance of control and speed, excellent for this design system
3. Other frameworks: If they can be heavily customized to match the iOS aesthetic
Implementation Priority
1. Remove Bootstrap completely - it conflicts with the iOS aesthetic
2. Choose one approach from the options above
3. Implement design system following this guide's principles
4. Maintain consistency across all components regardless of chosen method
Responsive Design
Breakpoints
Mobile: 320px - 768px
Tablet: 768px - 1024px
Desktop: 1024px+
Mobile-First Approach
Design for mobile first, then enhance for larger screens
Touch-friendly buttons (minimum 44px height)
Readable text sizes (minimum 16px on mobile)
Proper spacing for thumb navigation
Accessibility Requirements
Color Contrast
Text on background: Minimum 4.5:1 ratio
Interactive elements: Minimum 3:1 ratio
Never rely on color alone for information
Navigation
Keyboard navigation support
Focus indicators clearly visible
Skip links for screen readers
Semantic HTML structure
Interactive Elements
Alt text for all images
Form labels properly associated
Error messages descriptive and helpful
Loading states announced to screen readers
Animation & Micro-interactions
Subtle Animations
Hover states: 0.2s ease transitions
Page transitions: Smooth, not jarring
Loading animations: Skeleton screens preferred over spinners
Success feedback: Gentle confirmation animations
Performance
Respect motion preferences: prefers-reduced-motion
Hardware acceleration: Use transform and opacity for animations
60fps target: Keep animations smooth
Content Guidelines
Imagery
High-quality food photos when available
Consistent aspect ratios (preferably 16:9 or 4:3)
Placeholder images with food-related illustrations
Optimize for web: WebP format when supported
Iconography
Consistent icon style: Outline style preferred
16px and 24px sizes for most use cases
Meaningful icons that don't require explanation
Food and sustainability themed where appropriate
Messaging
Positive, encouraging tone about sustainability impact
Clear, actionable language for buttons and CTAs
Helpful error messages with next steps
Celebration of user impact (CO2 saved, food rescued, etc.)
Implementation Notes
CSS Framework Migration
1. Remove all Bootstrap classes from HTML
2. Choose your preferred approach:
Custom CSS: Implement following this guide's architecture
Tailwind CSS: Configure with the provided color palette and spacing scale
Other framework: Heavily customize to match iOS aesthetic
3. Implement layouts with chosen method (Grid/Flexbox/Framework utilities)
4. Build component library following design system principles
5. Test thoroughly across all supported browsers
Framework-Specific Configuration Examples
If using Tailwind CSS, configure your tailwind.config.js :
Performance Considerations
Critical CSS inlined for above-the-fold content
Progressive enhancement approach
Optimized images with proper lazy loading
Minimal JavaScript for interactions
Browser Support
javascript
mmoodduullee.eexxppoorrttss = {
theme: {
eexxtteenndd: {
ccoolloorrss: {
''pprriimmaarryy'': ''##3344CC775599'',
''pprriimmaarryy--ddaarrkk'': ''##3300BB0044FF'',
''aacccceenntt'': ''##EE88FF55EE88'',
''bbaacckkggrroouunndd'': ''##FF22FF22FF77'',
''tteexxtt--pprriimmaarryy'': ''##11CC11CC11EE'',
''tteexxtt--sseeccoonnddaarryy'': ''##88EE88EE9933'',
},
ssppaacciinngg: {
'xs': '4px',
'sm': '8px',
'md': ''1166ppxx'',
'lg': ''2244ppxx'',
'xl': ''3322ppxx'',
'xxl': ''4488ppxx'',
},
bboorrddeerrRRaaddiiuuss: {
'sm': '8px',
'md': ''1122ppxx'',
'lg': ''1166ppxx'',
}
}
}
}
Modern browsers (Chrome, Firefox, Safari, Edge)
Graceful degradation for older browsers
iOS Safari optimization (your target aesthetic)
Final Notes
This design system prioritizes:
1. User experience over flashy visuals
2. Accessibility as a core requirement
3. Performance and fast loading times
4. Sustainability messaging woven throughout
5. Mobile-first responsive design
The result should feel like a native iOS app translated to the web - clean, intuitive, and purposeful. Every
pixel should contribute to the user's ability to easily find and reserve food packages while feeling good
about their environmental impact.