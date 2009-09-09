\ ==============================================================================
\
\       gsv21_expl - the gtk-server aspect frame example in the ffl
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

\ Open the connection to the gtk-server and load all definitions from the gtk-server.cfg file

s" gtk-server.cfg" s" ffl-fifo" gsv+open 0= [IF]

  : event>widget   ( c-addr u -- n = Convert the event string to a widget id )
    0. 2swap >number 2drop d>s
  ;


  0 value window                \ Widgets/events


  : example
    gtk_init                    \ Initialise toolkit
    
    GTK_WINDOW_TOPLEVEL gtk_window_new to window
    
    s" Aspect Frame" window gtk_window_set_title
    
    10 window gtk_container_set_border_width
   
    \ Create an aspect_frame and add it to our toplevel window
   
    false 2.0E+0 0.5E+0 0.5E+0 s" 2x1" gtk_aspect_frame_new  >r 
   
    r@ window gtk_container_add
   
    \ Now add a child widget to the aspect frame
   
    gtk_drawing_area_new >r
   
    \ Ask for a 200x200 window, but the AspectFrame will give us a 200x100 window since we are forcing a 2x1 aspect ratio
    200 200 r@ gtk_widget_set_size_request
    r@ r'@ gtk_container_add
    r> gtk_widget_show
   
    r> gtk_widget_show            \ aspect frame
    
    window gtk_widget_show
 
                                  \ main loop
    BEGIN
      s" WAIT" gtk_server_callback
      event>widget window =       \ only one event
    UNTIL
    
    0 gtk_exit
  ;

  example
  
  gsv+close drop
    
[ELSE]
  .( No gtk-server fifo, is the gtk-server running ? ) cr
[THEN]


\ ==============================================================================
