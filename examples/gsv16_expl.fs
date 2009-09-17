\ ==============================================================================
\
\   gsv16_expl - the gtk-server color selection dialog example in the ffl
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

\ Note: requires gtk2.16

\ Open the connection to the gtk-server and load all definitions from the gtk-server.cfg file

s" gtk-server.cfg" s" ffl-fifo" gsv+open 0= [IF]

  0 value window                  \ Windows/events
  0 value color
  0 value color2
  0 value area
  0 value area-event
  0 value dialog
  0 value selection
  0 value color-changed


  : event>widget   ( c-addr u -- n = Convert the event string to a widget id )
    0. 2swap >number 2drop d>s
  ;


  \ callbacks
  
  \ Color changed handler
  : color_changed_cb  ( -- = process the color change event )
    dialog gtk_color_selection_dialog_get_color_selection
    
    color2 swap gtk_color_selection_get_current_color
    color2 GTK_STATE_NORMAL area gtk_widget_modify_bg
  ;

  
  \ Drawingarea event handler
  : area_event     ( -- = show the color selection dialog )
    dialog 0= IF
      \ Create color selection dialog
      s" Select background color" gtk_color_selection_dialog_new to dialog
    THEN

    \ Get the ColorSelection widget 
    dialog gtk_color_selection_dialog_get_color_selection to selection

    color selection gtk_color_selection_set_previous_color
    color selection gtk_color_selection_set_current_color
    true  selection gtk_color_selection_set_has_palette

    16 g_malloc to color-changed  \ Reserve event handle
    
    \ Connect to the "color_changed" signal
    color-changed s" color_changed" selection gsv+server-connect 2drop

    \ Show the dialog
    dialog gtk_dialog_run 

    GTK_RESPONSE_OK = IF
      color selection gtk_color_selection_get_current_color
    ELSE
      color GTK_STATE_NORMAL area gtk_widget_modify_bg
    THEN
      
    dialog gtk_widget_hide
  ;


  : example
    gtk_init                      \ Initialise toolkit
    
    GTK_WINDOW_TOPLEVEL gtk_window_new to window

    s" Color selection test" window gtk_window_set_title
    true true true window gtk_window_set_policy

    \ Create drawingarea, set size and catch button events

    gtk_drawing_area_new to area

    16 g_malloc to color
    16 g_malloc to color2
    
    color s" #00ff00" gdk_color_parse \ green
    
    color GTK_STATE_NORMAL area gtk_widget_modify_bg

    200 200 area gtk_widget_set_size_request

    GDK_BUTTON_PRESS_MASK area gtk_widget_set_events

    16 g_malloc to area-event     \ Reserve event handle
    
    area-event s" event" area gsv+server-connect 2drop
  
    \ Add drawingarea to window, then show them both
    area window gtk_container_add
    area gtk_widget_show

    window gtk_widget_show_all
                                  \ main loop
    BEGIN
      s" WAIT" gtk_server_callback
      event>widget CASE
        window        OF true ENDOF
        area-event    OF area_event       false ENDOF
        color-changed OF color_changed_cb false ENDOF
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
