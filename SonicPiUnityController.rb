'''
--------------------------------------------------------
Sonic Pi graphical user interface using Unity
--------------------------------------------------------
by Gonzalo María Cidoncha Pérez and Mario Tabasco Vargas
Version 0.0
'''

require 'ostruct'

'''
============================
  Attributes Struct
============================
'''
Attributes = Struct.new :synth_name, :fx_name, :sample_name, :sleep_dur, :note, :amp, :pan do
  # Initializes each attribute to its default value
  def initialize(*)
    super
    self.synth_name ||= 'beep'
    self.fx_name ||= 'echo'
    self.sample_name ||= 'bd_pure'
    self.sleep_dur ||= 1
    self.note ||= 52
    self.amp ||= 1
    self.pan ||= 0
  end
end

'''
============================
  Command Struct
============================
  Parameters:
  - loop_id: The index of the loop to which the command is attached
- com_id: The index of the command inside the loop command list
- com_name: The string containing the name of the command
- com_attr: List of attributes (pairs of <name, value>) of the command (i.e. <amp, 0.7>)

'''

Command = Struct.new(:loop_id, :com_id, :com_name, :com_attr) do
  # Initialize
  
  # Getters
  def getName
    com_name
  end
  
  def getLoopId
    loop_id
  end
  
  def getCommandId
    com_id
  end
  
  def getAttributes
    com_attr
  end
  
end

'''
============================
  Methods
============================
'''

'''
Waits for messages from Unity and adds the received commands
- id: Index of the listening loop
- commands: The list of commands of the listening loop
'''
def listenUnityCommand(id, commands)
  # Gets OSC message from Unity
  val = sync"/osc*/sonicpi/unity/trigger"
  
  if val[0] == "stop"
    comAttr = Attributes.new()
    com = Command.new(0, 0, "stop", comAttr)
    puts "APPLICATION STOPPED"
  else
    # Parse message to Command
    loopId = val[0]
    comId = val[1]
    comName = val[2]
    comAttr = Attributes.new(val[3], val[4], val[5], val[6], val[7], val[8], val[9])
    #puts comAttr
    
    com = Command.new(loopId, comId, comName, comAttr)
  end
  
  # Add the command to command list
  commands.push(com)
end

'''
Process the command list
- id: Index of the processing loop
- commands: The list of commands of the processing loop
'''
def processCommands(id, commands)
  # Processes each command from the command list in order
  commands.each do |com|
    puts "Processing: " + com.com_name
    
    case com.com_name
    # ACTION: SLEEP
    when "sleep"
      sleep com.com_attr.sleep_dur
      # ACTION: PLAY SYNTH
    when "synth"
      use_synth com.com_attr.synth_name
      play com.com_attr.note, amp: com.com_attr.amp, pan: com.com_attr.pan
      # STOP
    when "stop"
      stop
    else
      puts "ERROR: Unknown command name. Can't process command."
    end
  end
  sleep 0.1
end


'''
============================
  Listen/Play loops definition
============================
'''

def doListenerLoop(id,commands)
  puts "Listener " + id.to_s
  # LISTENER LOOP
  in_thread do
    loop do
      #puts "id = " + id.to_s
      # Listen commands for command list 'id'
      listenUnityCommand(id, commands[id])
      puts "Listening (" + id.to_s + ")..."
      sleep 0.1
    end
  end
end

def doPlayerLoop(id,commands)
  puts "Player " + id.to_s
  # Set the live_loop name
  loopName = "playerLoop#{id.to_s}"
  # PLAYER LOOP
  live_loop loopName do
    puts "Playing (" + id.to_s + ")..."
    # Process command list number 'id'
    processCommands(id, commands[id])
    sleep 0.1
  end
end


'''
========================================================
  Variables and Initialization of the loop rack
========================================================
'''
use_osc "localhost", 4560

# Number of loops listening to Unity commands
NUM_LOOPS = 1
# Command list
commands = []

# Command list initialization
for i in 0..(NUM_LOOPS - 1)
  puts i
  commands[i] = []
  doListenerLoop(i, commands) # Starts listening loop
  doPlayerLoop(i, commands)   # Starts playing loop
end


'''
live_loop :listener1 do
  # Almacena en val lo que recibe por OSC desde Unity
  val = sync"/osc*/sonicpi/unity/trigger"
  puts "Received command " + val[0]
  # Lo añade al array de acciones
  actions.push(val)
  puts actions
end

live_loop :player1 do
  # Reproduce las acciones del array actions en orden
  actions.each do |action|
    puts action[0]
    case action[0]
    when "sleep"
      sleep 1
    when "play"
      play 89, amp: 0.1
    when "stop"
      stop
    else
      puts "ERROR"
    end
  end
  sleep 0.1
end
'''


