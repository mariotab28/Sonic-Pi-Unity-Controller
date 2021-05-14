'''
--------------------------------------------------------
Sonic Pi graphical user interface using Unity
--------------------------------------------------------
by Gonzalo Maria Cidoncha Perez and Mario Tabasco Vargas
Version 0.0
'''

require 'ostruct'

'''
============================
  Attributes Structures
============================
'''

''' Attribute structure of the LOOPS '''
LoopAttributes = Struct.new :active, :synced_with, :bpm do
  # Initializes each attribute to its default value
  def initialize(*)
    super
    self.active ||= 1
    self.synced_with ||= ''
    self.bpm ||= 60
  end
end

''' Attribute structure of the SYNTH play action '''
SynthAttributes = Struct.new :action, :synth_name, :notes, :mode, :amp, :pan,
:attack, :decay, :sustain, :release, :attack_level, :decay_level, :sustain_level, :fx do
  # Initializes each attribute to its default value
  def initialize(*)
    super
    self.action ||= 'synth'
    self.synth_name ||= 'beep'
    self.notes ||= [52]
    self.mode ||= 'tick'
    self.amp ||= 1
    self.pan ||= 0
    self.attack ||= 0
    self.sustain ||= 0
    self.release ||= 1
    self.decay ||= 0
    self.attack_level ||= 1
    self.sustain_level ||= 1
    self.decay_level ||= self.sustain_level
    # Effect
    self.fx ||= ''
  end
end

''' Attribute structure of the SAMPLE play action '''
SampleAttributes = Struct.new :action, :sample_name, :amp, :pan,
  :attack, :decay, :sustain, :release, :attack_level, :decay_level, :sustain_level,
  :lpf, :lpf_attack, :lpf_decay, :lpf_sustain, :lpf_release, :lpf_min, :lpf_init_level, :lpf_release_level, :lpf_sustain_level, :lpf_decay_level, :lpf_attack_level, :lpf_env_curve,
  :hpf, :hpf_max, :hpf_attack, :hpf_decay, :hpf_sustain, :hpf_release, :hpf_init_level, :hpf_release_level, :hpf_sustain_level, :hpf_decay_level, :hpf_attack_level, :hpf_env_curve,
  :rate, :start, :finish, :norm, :pitch, :window_size, :pitch_dis, :time_dis, :compress, :threshold, :clamp_time,
:slope_above, :slope_below, :relax_time, :fx do
  # Initializes each attribute to its default value
  def initialize(*)
    super
    self.action ||= 'sample'
    self.sample_name ||= 'bd_pure'
    self.amp ||= 1
    self.pan ||= 0
    self.attack ||= 0
    self.sustain ||= -1
    self.release ||= 1
    self.decay ||= 0
    self.attack_level ||= 1
    self.sustain_level ||= 1
    self.decay_level ||= self.sustain_level
    # Sample configuration
    # Low Pass Filter
    self.lpf ||= -1
    self.lpf_attack ||= self.attack
    self.lpf_decay ||= self.decay
    self.lpf_sustain ||= self.sustain
    self.lpf_release ||= self.release
    self.lpf_min||= 130
    self.lpf_init_level ||= self.lpf_min
    self.lpf_release_level ||= self.lpf
    self.lpf_sustain_level ||= self.lpf_release_level
    self.lpf_decay_level ||= self.lpf_sustain_level
    self.lpf_attack_level ||= self.lpf_decay_level
    self.lpf_env_curve ||= 2
    # High Pass Filter
    self.hpf ||= -1
    self.hpf_max ||= 200
    self.hpf_attack ||= self.attack
    self.hpf_decay ||= self.decay
    self.hpf_sustain ||= self.sustain
    self.hpf_release ||= self.release
    self.hpf_init_level ||= 130
    self.hpf_release_level ||= self.hpf
    self.hpf_sustain_level ||= self.hpf_release_level
    self.hpf_decay_level ||= self.hpf_sustain_level
    self.hpf_attack_level ||= self.hpf_decay_level
    self.hpf_env_curve ||= 2
    # Other sample attributes
    self.rate ||= 1
    self.start ||= 0
    self.finish ||= 1
    self.norm ||= 0
    self.pitch ||= 0
    self.window_size ||= 0.2
    self.pitch_dis ||= 0.0
    self.time_dis ||= 0.0
    self.compress ||= 0
    self.threshold ||= 0.2
    self.clamp_time ||= 0.01
    self.slope_above ||= 0.5
    self.slope_below ||= 1
    self.relax_time ||= 0.01
    # Effect    
    self.fx ||= ''
  end
end

''' Attribute structure of the SLEEP action '''
SleepAttributes = Struct.new :action, :sleep_duration do
  # Initializes each attribute to its default value
  def initialize(*)
    super
    self.action ||= 'sleep'
    self.sleep_duration ||= 1
  end
end


'''
============================
  Command Struct
============================
  Parameters:
  - loop_id: The index of the loop to which the command is attached
- com_id: The index of the command inside the loop command list
- com_attr: Structure containing the attributes of the command

'''

Command = Struct.new(:loop_id, :com_id, :com_attr) do
  # Initialize
  def initialize(*)
    super
    self.loop_id ||= -1
    self.com_id ||= -1
    self.loop_id ||= nil
  end
end

'''
============================
  Parsing Methods
============================
'''

''' Parses message contained in val and return a sleep command attribute structure'''
def parseSleepCommand(val, i)
  return SleepAttributes.new(val[i], val[i+1])
end

''' Parses message contained in val and return a synth command attribute structure'''
def parseSynthCommand(val, pre_i, i, notesToPlay)
  comAttr = SynthAttributes.new()
  comAttr.action = val[pre_i]
  comAttr.synth_name = val[pre_i + 1]
  comAttr.notes = notesToPlay
  comAttr.mode = val[i]
  comAttr.amp = val[i + 1]
  comAttr.pan = val[i + 2]
  comAttr.attack = val[i + 3]
  comAttr.sustain = val[i + 4]
  comAttr.release = val[i + 5]
  comAttr.decay = val[i + 6]
  comAttr.attack_level = val[i + 7]
  comAttr.sustain_level = val[i + 8]
  comAttr.decay_level = val[i + 9]
  comAttr.fx = val[i + 10]
  return comAttr
end

''' Parses message contained in val and return a sample command attribute structure'''
def parseSampleCommand(val, i)
  comAttr = SampleAttributes.new()
  comAttr.action = val[i]
  comAttr.sample_name = val[i + 1]
  comAttr.amp = val[i + 2]
  comAttr.pan = val[i + 3]
  comAttr.attack = val[i + 4]
  comAttr.sustain = val[i + 5]
  comAttr.release = val[i + 6]
  comAttr.decay = val[i + 7]
  comAttr.attack_level = val[i + 8]
  comAttr.sustain_level = val[i + 9]
  comAttr.decay_level = val[i + 10]
  comAttr.lpf = val[i + 11]
  comAttr.lpf_attack = val[i + 12]
  comAttr.lpf_decay = val[i + 13]
  comAttr.lpf_sustain = val[i + 14]
  comAttr.lpf_release = val[i + 15]
  comAttr.lpf_min = val[i + 16]
  comAttr.lpf_init_level = val[i + 17]
  comAttr.lpf_release_level = val[i + 18]
  comAttr.lpf_sustain_level = val[i + 19]
  comAttr.lpf_decay_level = val[i + 20]
  comAttr.lpf_attack_level = val[i + 21]
  comAttr.lpf_env_curve = val[i + 22]
  comAttr.hpf = val[i + 23]
  comAttr.hpf_max = val[i + 24]
  comAttr.hpf_attack = val[i + 25]
  comAttr.hpf_decay = val[i + 26]
  comAttr.hpf_sustain = val[i + 27]
  comAttr.hpf_release = val[i + 28]
  comAttr.hpf_init_level = val[i + 29]
  comAttr.hpf_release_level = val[i + 30]
  comAttr.hpf_sustain_level = val[i + 31]
  comAttr.hpf_decay_level = val[i + 32]
  comAttr.hpf_attack_level = val[i + 33]
  comAttr.hpf_env_curve = val[i + 34]
  comAttr.rate = val[i + 35]
  comAttr.start = val[i + 36]
  comAttr.finish = val[i + 37]
  comAttr.norm = val[i + 38]
  comAttr.pitch = val[i + 39]
  comAttr.window_size = val[i + 40]
  comAttr.pitch_dis = val[i + 41]
  comAttr.time_dis = val[i + 42]
  comAttr.compress = val[i + 43]
  comAttr.threshold = val[i + 44]
  comAttr.clamp_time = val[i + 45]
  comAttr.slope_above = val[i + 46]
  comAttr.slope_below = val[i + 47]
  comAttr.relax_time = val[i + 48]
  comAttr.fx = val[i + 49]
  return comAttr
end

'''
Process a synth command and applies its attributes
'''
def playSynthCommand(tickNote, com)
  play tickNote, amp: com.com_attr.amp, pan: com.com_attr.pan,
    attack: com.com_attr.attack, sustain: com.com_attr.sustain, release: com.com_attr.release,
    decay: com.com_attr.decay, attack_level: com.com_attr.attack_level, sustain_level: com.com_attr.sustain_level,
    decay_level: com.com_attr.decay_level
end

'''
Process a synth command and applies its attributes
'''
def playSampleCommand(com)
  sample com.com_attr.sample_name, amp: com.com_attr.amp, pan: com.com_attr.pan, attack: com.com_attr.attack, sustain: com.com_attr.sustain, release: com.com_attr.release, decay: com.com_attr.decay,
    attack_level: com.com_attr.attack_level, sustain_level: com.com_attr.sustain_level, decay_level: com.com_attr.decay_level,
    lpf: com.com_attr.lpf,
    lpf_attack: com.com_attr.lpf_attack,
    lpf_decay: com.com_attr.lpf_decay,
    lpf_sustain: com.com_attr.lpf_sustain,
    lpf_release: com.com_attr.lpf_release,
    lpf_min: com.com_attr.lpf_min,
    lpf_init_level: com.com_attr.lpf_init_level,
    lpf_release_level: com.com_attr.lpf_release_level,
    lpf_sustain_level: com.com_attr.lpf_sustain_level,
    lpf_decay_level: com.com_attr.lpf_decay_level,
    lpf_attack_level: com.com_attr.lpf_attack_level,
    lpf_env_curve: com.com_attr.lpf_env_curve,
    hpf: com.com_attr.hpf,
    hpf_max: com.com_attr.hpf_max,
    hpf_attack: com.com_attr.hpf_attack,
    hpf_decay: com.com_attr.hpf_decay,
    hpf_sustain: com.com_attr.hpf_sustain,
    hpf_release: com.com_attr.hpf_release,
    hpf_init_level: com.com_attr.hpf_init_level,
    hpf_release_level: com.com_attr.hpf_release_level,
    hpf_sustain_level: com.com_attr.hpf_sustain_level,
    hpf_decay_level: com.com_attr.hpf_decay_level,
    hpf_attack_level: com.com_attr.hpf_attack_level,
    hpf_env_curve: com.com_attr.hpf_env_curve,
    rate: com.com_attr.rate,
    start: com.com_attr.start,
    finish: com.com_attr.finish,
    norm: com.com_attr.norm,
    pitch: com.com_attr.pitch,
    window_size: com.com_attr.window_size,
    pitch_dis: com.com_attr.pitch_dis,
    time_dis: com.com_attr.time_dis,
    compress: com.com_attr.compress,
    threshold: com.com_attr.threshold,
    clamp_time: com.com_attr.clamp_time,
    slope_above: com.com_attr.slope_above,
    slope_below: com.com_attr.slope_below,
    relax_time: com.com_attr.relax_time
end

'''
============================
  Listen/Process Methods
============================
'''

'''
Displays all the content of val for debugging purposes
'''
def printMessageValues(val)
  len = val.length()
  for i in 0..(len-1) do
    puts i.to_s + "| " + val[i].to_s
  end
end

'''
Waits for messages from Unity and adds the received commands
- id: Index of the listening loop
- commands: The list of commands of the listening loop
'''
def listenUnityCommand(id, commands, loops)
  use_osc "localhost", 4560
  # Gets OSC message from Unity
  val = sync"/osc*/sonicpi/unity/trigger"

  if val[0] == "del_loop"
    return 0
  end

  if val[0] == "stop"
    comAttr = SleepAttributes.new("stop", -1)
    comId = 0
    com = Command.new(0, 0, comAttr)
    puts "Loop stopped"
    
    # Add the command to command list
    commands[comId] = com
  else

    '''
    Structure of the message:
    * numLoops, 
      <for each loop:>
      * loopId, numberOfValues, numberOfCommands,
        <for each command:>
        * commandId, commandName, <list of values for each attribute>
    '''
    numLoops = val[0]
    if numLoops <= 0
      puts "Wrong number of loops."
      return -1
    end
    
    a = 1 # Index to run through message values
    thisLoop = false
    l = 0
    while (l < numLoops) and (not thisLoop) do
      loopId = val[a]
      # Is this my loop?
      if loopId == id
        thisLoop = true
      elsif
        # Not this loop => skip: number of values, number of commands and  message values
        numVals = val[a + 1]
        a = a + 1 + 1 + numVals + 1
      end
      l = l + 1 # Next loop 
    end
    # Exit if there are no commands for this loop
    if not thisLoop
      puts "No messages for loop " + id.to_s
      return 0
    end
      
    printMessageValues(val)
    a = a + 1 + 1 # Skip numVals
    # Run through each command values
    numComs = val[a]
    puts " * Parsing " + numComs.to_s + " commands for loop " + id.to_s
    a = a + 1 # Skip numComs
    for k in 1..numComs do
      # Parse message to Command
      comId = val[a]
      i = a + 1 # Skip comId
      puts "Received Command: " + val[i].to_s + "(at " + i.to_s + ")"
      command = true # Flag that indicates a change in the command list
      case val[i] # Check command name
      # COMMAND: CHANGE LOOP ATTRIBUTES
      when "loop"
        command = false # Don't change the command list
        # Set Loop attributes
        loops[id].active = val[i + 1]
        sync = val[i + 2]
        if sync >= 0 && sync < loops.length()
          loops[id].synced_with = "/live_loop/playerLoop" + sync.to_s 
        elsif sync == -1
          loops[id].synced_with = ""
        end
        loops[id].bpm = val[i + 3]
        nAttr = 3 
        # Skip: command name and attributes(1)
        a = a + 1 + nAttr + 1
      # ACTION: SLEEP
      when "sleep"
        comAttr = parseSleepCommand(val, i)
        nAttr = 1 
        # Skip: command name and attributes(1)
        a = a + 1 + nAttr + 1
        # ACTION: PLAY SYNTH
      when "synth"
        numOfNotes = val[i+2] # Number of notes of the sequence to play
        notesToPlay = [] # List of notes to play
        # Add the notes from the message to notesToPlay
        bi = i + 3 # Index of the beginning of the note sequence
        ei = i + 3 + numOfNotes - 1 # Index of the end of the note sequence
        
        for ni in bi..ei do notesToPlay.push(val[ni]) end
        
        pre_i = i
        i = ei + 1
        comAttr = parseSynthCommand(val, pre_i, i, notesToPlay)
        nAttr = 1 
        # Skip: command name + player name + number of notes 
        #   + numOfNotes + mode + attributes(9 + fx)
        a = a + 1 + 1 + 1 + numOfNotes + 1 + 10 + 1
        # SAMPLE
        when "sample"
          comAttr = parseSampleCommand(val, i)
          # Skip: command name + player name + attributes(47 + fx)
          a = a + 1 + 1 + 48 + 1
        when "empty"
          comAttr = SleepAttributes.new("empty", -1)
          # Skip: command name and attributes(1)
          a = a + 1 + 1
        else
          comAttr = nil
          puts "ERROR: Unknown action name."
      end
      
      if command
        com = Command.new(loopId, comId, comAttr)
        # Add the command to command list
        commands[comId] = com
      end
    end
  end
end

'''
Process the command list
- id: Index of the processing loop
- commands: The list of commands of the processing loop
'''
def processCommands(id, commands, loops)
  slept = false
  synced = false
  # Get loop attributes
  loop_active = loops[id].active
  loop_synced_with = loops[id].synced_with
  loop_bpm = loops[id].bpm
  # Do not process commands if loop is not active
  if loop_active != 1
    sleep 1
    return
  end
  # Either sync or use bpm
  if loop_synced_with != ''
    puts "Loop " + id.to_s + " syncing with " + loop_synced_with
    sync loop_synced_with
    synced = true
  else
    use_bpm loop_bpm
  end

  # Processes each command from the command list in order
  commands.each do |com|
    commandPlayed = false
    puts "Processing: " + com.com_attr.action.to_s
    case com.com_attr.action
    # ACTION: SLEEP
    when "sleep"
      # Send advance message to application
      use_osc "localhost", 5555
      osc "Application", "advance_loop", id, com.com_id

      sleep com.com_attr.sleep_duration
      if com.com_attr.sleep_duration > 0.001
        slept = true
      end
    # ACTION: PLAY SYNTH
    when "synth"
      if com.com_attr.notes.count > 0
        use_synth com.com_attr.synth_name
        if com.com_attr.mode == 'tick' then
          tickNote = com.com_attr.notes.tick
        elsif com.com_attr.mode == 'chord' then
          tickNote = com.com_attr.notes
        elsif com.com_attr.mode == 'choose' then
          tickNote = com.com_attr.notes.choose
        else
          puts "Error: Unknown synth play mode."
        end
        if com.com_attr.fx != ''
          with_fx com.com_attr.fx do
            playSynthCommand(tickNote, com)
          end
        else
          playSynthCommand(tickNote, com)
        end
      end
      commandPlayed = true
    # ACTION: PLAY SAMPLE
    when "sample"
      if com.com_attr.fx != ''
        with_fx com.com_attr.fx do
          playSampleCommand(com)
        end
      else
        playSampleCommand(com)
      end
      commandPlayed = true
      ''' TODO: RESTO DE ATRIBUTOS '''
      ''' ... '''
    when "empty"
      puts "Empty command in loop " + id.to_s
    # STOP
    when "stop"
      puts "Stop processing loop " + id.to_s
      stop
    else
      puts "ERROR: Unknown command name. Can't process command."
    end
    # Send advance message to application
    if commandPlayed
      use_osc "localhost", 5555
      osc "Application", "advance_loop", id, com.com_id
    end
  end
  # Check if there is one sleep at the end
  if !slept && !synced
    sleep 0.5 # Needs to sleep at least 0.01
  end
end
  
  
'''
============================
Listen/Play loops definition
============================
'''

def doListenerLoop(id,commands, loops)
  puts "Listener " + id.to_s
  # LISTENER LOOP
  in_thread do
    loop do
      #puts "Listening (" + id.to_s + ")..."
      # Listen commands for command list 'id'
      listenUnityCommand(id, commands[id], loops)
      sleep 0.1
    end
  end
end

def doPlayerLoop(id,commands, loops)
  puts "Player " + id.to_s
  # Set the live_loop name
  loopName = "playerLoop#{id.to_s}"
  # PLAYER LOOP
  live_loop loopName do
    #puts "Playing (" + id.to_s + ")..."
    # Process command list number 'id'
    processCommands(id, commands[id], loops)
  end
end


'''
========================================================
Variables and Initialization of the loop rack
========================================================
'''
use_osc "localhost", 4560

puts "========================================================"
puts "Starting Sonic Pi controller."
puts "You can start the application now."
puts "========================================================"

# Command list
commands = []
# Loop attributes list
loops = []

def deleteLoop(id, commands)
  # Remove loop[id]
  puts "Removing loop with id: " + id.to_s
  commands[id] = []
end

# Number of loops listening to Unity commands
nLoops = 0

while nLoops <= 0 do
  val = sync"/osc*/sonicpi/unity/trigger"
  if val[0] == "init"
    nLoops = val[1]
  end
end

# Creates rack of loops
for i in 0..(nLoops - 1)
  puts "Creating loop " + i.to_s
  commands[i] = []
  loopAttr = LoopAttributes.new
  loops[i] = loopAttr
  doListenerLoop(i, commands, loops) # Starts loop listening thread
  doPlayerLoop(i, commands, loops)   # Starts loop playing thread
end

  

