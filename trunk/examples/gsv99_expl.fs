\ ==============================================================================
\
\       gsv99_expl - the gtk-server mandelbrot example in the ffl
\
\               Copyright (C) 2009  Dick van Oudheusden
\  
\ This library is free software; you can redistribute it and/or
\ modify it under the terms of the GNU General Public
\ License as published by the Free Software Foundation; either
\ version 2 of the License, or (at your option) any later version.
\
\ This library is distributed in the hope that it will be useful,
\ but WITHOUT ANY WARRANTY; without even the implied warranty of
\ MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
\ General Public License for more details.
\
\ You should have received a copy of the GNU General Public
\ License along with this library; if not, write to the Free
\ Software Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
\
\ ==============================================================================
\ 
\  $Date: 2009-05-28 17:35:58 $ $Revision: 1.11 $
\
\ ==============================================================================

include ffl/gsv.fs
include ffl/spf.fs

\ This example is inspired by the fractal.ksh file for the gtk stuff and
\ http://warp.povusers.org/Mandelbrot/ for the mandelbrot algorithm and coloring.

\ Open the connection to the gtk-server and load all definitions from the gtk-server.cfg file

s" gtk-server.cfg" s" ffl-fifo" gsv+open 0= [IF]

  : event>widget   ( c-addr u -- n = Convert the event string to a widget id )
    0. 2swap >number 2drop d>s
  ;


  0 value window                  \ Widgets/events
  0 value draw-button
  0 value clear-button
  0 value exit-button
  0 value graphics
  0 value image
  0 value pixmap
  0 value event-box
  0 value clear-text
  0 value start-text
  0 value end-text

 16 value color-size
  0 value color
  0 value white-color
  0 value black-color

  300 constant width
  240 constant height
  32  constant colors
  32  constant iterations     \ Iterations should be a multiply of colors
  
  iterations colors /
      constant iter/color
      
    -2E+0 fconstant minRe
     1E+0 fconstant maxRe
  -1.2E+0 fconstant minIm
  
  maxRe minRe f- height s>d d>f f* width s>d d>f f/ minIm f+
          fconstant maxIm
  maxRe minRe f- width 1- s>d d>f f/
          fconstant fRe
  maxIm minIm f- height 1- s>d d>f f/
          fconstant fIm
          
  
  \ Callbacks
  : draw-fractal   ( -- flag = Draw the fractal )
    \ Tell calculation is starting
    black-color graphics gdk_gc_set_rgb_fg_color
    start-text 240 10 graphics pixmap gdk_draw_layout
    image gtk_widget_queue_draw
    s" UPDATE" gtk_server_callback 2drop
    
    \ Calculations
    height 0 DO
      maxIm I s>d d>f fIm f* f-        \ S: cIm
      width 0 DO
        minRe I s>d d>f fRe f* f+      \ S: cIm cRe
        fover fover                    \ S: cIm cRe Zim Zre
        true
        iterations 0 DO
          fover fover
          fdup f* fswap fdup f*        \ S: cIm cRe Zim Zre Zre^2 Zim^2
          fover fover f+ 4E+0 f> IF    \ If Zre^2 + Zim^2 > 4.0
            fdrop fdrop
            drop I false               \   Done, return iterations
            leave
          THEN
          f-                           \ S: cIm cRe Zim Zre Zre^2-Zim^2
          f>r 2E+0 f* f*               \ S: cIm cRe 2.0*Zre*Zim
          f>r fover fr> f+             \ S: cIm cRe 2.0*Zre*Zim+CIm
          fover fr> f+                 \ S: cIm cRe 2.0*Zre*Zim+CIm Zre^2-Zim^2+cRe
        LOOP
        fdrop fdrop
        IF
          black-color
        ELSE
          iter/color / color-size * color +
        THEN
        graphics gdk_gc_set_rgb_fg_color
        J I graphics pixmap gdk_draw_point
        
        s" UPDATE" gtk_server_callback event>widget 
        dup window = swap exit-button = OR IF
          fdrop fdrop
          unloop unloop
          true exit
        THEN
        fdrop
      LOOP
      fdrop
    LOOP
    
    \ Wipe wait text
    white-color graphics gdk_gc_set_rgb_fg_color
    25 120 240 10 1 graphics pixmap gdk_draw_rectangle
    \ Tell drawing is ready
    black-color graphics gdk_gc_set_rgb_fg_color
    end-text 240 10 graphics pixmap gdk_draw_layout
    image gtk_widget_queue_draw
    s" UPDATE" gtk_server_callback event>widget
    dup window = swap exit-button = OR
  ;
  
  
  : clear-fractal  ( -- = Clear the fractal )
    white-color graphics gdk_gc_set_rgb_fg_color 
    265 340 0 0 1 graphics pixmap gdk_draw_rectangle 
    black-color graphics gdk_gc_set_rgb_fg_color 
    clear-text 240 130 graphics pixmap gdk_draw_layout 
    image gtk_widget_queue_draw 
  ;


  : color!+        ( color1 c-addr u -- color2 = Store color c-addr u in color address )
    2>r dup 2r> gdk_color_parse color-size +
  ;


  : setup-colors   ( -- = Setup the color array )
    color
    s" #100000" color!+
    s" #200000" color!+
    s" #300000" color!+
    s" #400000" color!+
    s" #500000" color!+
    s" #600000" color!+
    s" #700000" color!+
    s" #800000" color!+
    s" #900000" color!+
    s" #a00000" color!+
    s" #b00000" color!+
    s" #c00000" color!+
    s" #d00000" color!+
    s" #e00000" color!+
    s" #f00000" color!+
    s" #ff0000" color!+
    s" #ff1010" color!+
    s" #ff2020" color!+
    s" #ff3030" color!+
    s" #ff4040" color!+
    s" #ff5050" color!+
    s" #ff6060" color!+
    s" #ff7070" color!+
    s" #ff8080" color!+
    s" #ff9090" color!+
    s" #ffa0a0" color!+
    s" #ffb0b0" color!+
    s" #ffc0c0" color!+
    s" #ffd0d0" color!+
    s" #ffe0e0" color!+
    s" #fff0f0" color!+
    s" #ffffff" color!+
    drop
  ;


  : example
    gtk_init                    \ Initialise toolkit
    
    \ Main Window
    GTK_WINDOW_TOPLEVEL gtk_window_new to window
    
    s" Forth with GTK-server" window gtk_window_set_title
    
    300 300 window gtk_widget_set_size_request
    1 window gtk_window_set_position
    0 window gtk_window_set_resizable

    0 0 gtk_vbox_new >r
    
    r@ window gtk_container_add 
    
    \ Display image
    gtk_image_new to image

    \ Event box for mouse click
    gtk_event_box_new to event-box
    
    image event-box gtk_container_add

    1 0 0 event-box r@ gtk_box_pack_start 
    
    \ Seperator
    1 0 0 gtk_hseparator_new r@ gtk_box_pack_start 
    
    \ Button box
    
    0 0 gtk_hbox_new >r
    
    s" Clear" gtk_button_new_with_label to clear-button
    
    30 75 clear-button gtk_widget_set_size_request
    
    1 0 0 clear-button r@ gtk_box_pack_start
    
    s" Draw!" gtk_button_new_with_label to draw-button
    
    30 75 draw-button gtk_widget_set_size_request
    
    1 0 0 draw-button r@ gtk_box_pack_start
    
    s" Exit" gtk_button_new_with_label to exit-button
    
    30 75 exit-button gtk_widget_set_size_request
    
    1 0 0 exit-button r@ gtk_box_pack_end
    
    \ Put the button box in the vbox
    1 0 0 r> r> gtk_box_pack_end 

    \ show all
    window gtk_widget_show_all 

    \ Create pixmap
\    -1 265 300  image gtk_widget_get_parent_window  gdk_pixmap_new to pixmap
     24 265 300  image gtk_widget_get_parent_window  gdk_pixmap_new to pixmap
    
    \ Create GC
    pixmap gdk_gc_new to graphics
    
    pixmap image gtk_image_set_from_pixmap 
    
    \ Allocate the color
    color-size g_malloc to white-color
    color-size g_malloc to black-color
    color-size colors * g_malloc to color
    
    \ Setup the color array
    setup-colors
    
    \ Set fore- and background to white
    white-color s" #ffffff" gdk_color_parse 
    
    white-color graphics gdk_gc_set_rgb_bg_color
    white-color graphics gdk_gc_set_rgb_fg_color 
    
    \ Clear the pixmap
    265 300 0 0 1 graphics pixmap gdk_draw_rectangle 
    
    \ Black color
    black-color s" #000000" gdk_color_parse 

    black-color graphics gdk_gc_set_rgb_fg_color 
    
    \ Put some text on the canvas
    s" Draw a fractal with Forth!" image gtk_widget_create_pango_layout to clear-text
    
    clear-text 240 130 graphics pixmap gdk_draw_layout 
    
    \ Define start and finishing text
    s" Please wait..." image gtk_widget_create_pango_layout to start-text
    s" Drawing ready"  image gtk_widget_create_pango_layout to end-text

    \ Update the image with the pixmap
    image gtk_widget_queue_draw 
                         
    BEGIN                         \ main loop
      s" WAIT" gtk_server_callback
      event>widget CASE
        window        OF true ENDOF
        exit-button   OF true ENDOF
        draw-button   OF draw-fractal          ENDOF
        clear-button  OF clear-fractal   false ENDOF
      false swap
      ENDCASE
    UNTIL
    
    0 gtk_exit
  ;

  example
  
  gsv+close drop
    
[ELSE]
  .( No gtk-server fifo, is the gtk-server running ? ) cr
[THEN]


\ ==============================================================================
